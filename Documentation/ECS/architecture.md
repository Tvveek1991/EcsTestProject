# Целевая ECS-модель

## Решение

**LeoEcsLite остаётся ECS-ядром игры.** Он владеет игровыми сущностями,
компонентами и правилами, определяющими состояние игры. Переход на Unity DOTS
не является целью этой архитектуры.

Unity является внешним слоем представления и интеграции:

- `GameObject`, `Transform`, `SpriteRenderer`, UI и `Animator` отображают
  состояние ECS;
- `Rigidbody2D`, `Collider2D` и Unity callbacks служат мостом к Unity 2D
  physics;
- DOTween воспроизводит presentation-анимации и эффекты;
- загрузка префабов, создание и удаление view выполняются bridge-слоем.

Этот слой не является источником игрового состояния. Игровые правила не должны
зависеть от случайного порядка Unity callbacks, существования конкретного
`GameObject` или завершения tween.

## Границы ответственности

| Слой | Владеет | Не делает |
| --- | --- | --- |
| ECS components | Состоянием игры и краткоживущими командами/событиями. | Не хранит неявные правила во view. |
| Input bridge | Читает `GameplayInputActions` через Unity Input System и обновляет один snapshot перед ECS tick. | Не применяет игровой эффект напрямую и не создаёт gameplay-команды. |
| Simulation systems | Правилами игры: движение как намерение, здоровье, состояния, награды, смерть. | Не вызывает Unity API, DOTween, `Instantiate` или `Destroy`. |
| Physics bridge | Синхронизацией намерений и результатов с `Rigidbody2D`/`Collider2D` на fixed tick. | Не принимает решения о правилах игры вне явно опубликованных bridge-событий. |
| Presentation bridge | Созданием, обновлением и удалением view; Animator, UI, камера, эффекты и DOTween. | Не меняет gameplay-состояние как побочный эффект отображения. |
| Cleanup | Удалением временных компонентов, view и завершённых сущностей. | Не оставляет неописанных владельцев ресурсов. |

Unity-ссылки в ECS допустимы только как явно названные bridge-компоненты:
`*ViewRef`, `*Binding` или `*Bridge`. Обычный компонент хранит только игровое
состояние и данные, нужные simulation-системам.

## Entity view registry

`EntityView` — базовый MonoBehaviour для view, принадлежащих ECS-сущностям.
При первой регистрации он получает `EntityLink`; link хранит entity и
идентификатор scoped registry. `IEntityViewRegistry` является частью игровой
сессии и предоставляет только typed `Register`, `TryGet`, `TryGetEntity` и
`Unregister`: gameplay-системы не получают mutable dictionary view.

Для физического bridge registry также хранит scoped-связь каждого
`Collider2D` view с entity. После `Physics2D.Raycast` система получает entity
одним `TryGetEntity(collider, ...)`, без обхода всех объектов; обратный индекс
снимает все collider-связи при `Unregister` и teardown.

`HealthViewFollowSystem` проходит только владельцев `Health`, получает их
конкретный `HealthView` по `ViewEntity` и обновляет позицию от точки owner view;
попарный обход health view и владельцев не допускается.

Повторная регистрация entity, попытка использовать view другой сессии и
устаревший или уничтоженный view не возвращают произвольную ссылку. При
teardown scoped registry очищает связи; системы удаления сначала снимают
регистрацию, затем уничтожают GameObject и ECS-entity.

`IGameplayViewFactory` — scoped presentation bridge для создания `PersonView`,
`ObjectView`, `CoinView` и `HealthView`. ECS-системы запрашивают typed view у
фабрики, регистрируют его и настраивают presentation-state, но не вызывают
`Object.Instantiate` напрямую.

`IGameplayTweenRegistry` — scoped owner всех gameplay tween и sequence. Он
передаётся в `GameSession` как external operation: при teardown сначала
отменяется token, затем registry убивает отслеживаемые tween, и только после
этого уничтожаются ECS systems и world. Любой callback, который пишет в ECS,
выполняется через `TryExecute`, поэтому после отмены сессии он не обращается к
уже уничтоженному миру.

## Фазы игрового цикла

```text
однократная инициализация
          |
  Update: Input -> Simulation -> Presentation -> Cleanup
                     |
  FixedUpdate: Physics bridge
```

| Фаза | Такт | Вход | Выход |
| --- | --- | --- | --- |
| Initialization | однократно на сессию | конфигурация и prefabs | сущности и начальные bindings |
| Input | `Update` | Unity input | snapshot и команды текущего кадра |
| Simulation | `Update` | состояние и input-команды | новое gameplay-состояние, намерения и gameplay-события |
| Physics | `FixedUpdate` | физические намерения и Unity 2D physics | данные контактов и физические события для следующей simulation-фазы |
| Presentation | `Update`/`LateUpdate` | gameplay-состояние и события | состояние view, UI, Animator и tween |
| Cleanup | в конце `Update` | одноразовые команды, удаление сущностей | очищенный мир и освобождённые view |

Порядок систем внутри каждой фазы объявляется в одном composer/installer, а не
вытекает из порядка разрешения `IEnumerable<IEcsSystem>` контейнером.

На переходном этапе `GameSystemsInstaller` уже регистрирует шесть явных групп
в порядке `Initialization → Input → Simulation → Physics → Presentation →
Cleanup`, но они пока исполняются одним `Update` tick. `GameSystemsComposer`
явно сопоставляет зарегистрированные instances с таблицей system types и
отклоняет отсутствующие, дублированные или неописанные системы. Отдельные Unity
ticks для physics/presentation будут добавлены только после фиксации правил
передачи команд между фазами.

Перед каждым `GameEcsLoop.Tick()` Unity input bridge обновляет
`GameplayInputSnapshot`; `InputSystem` копирует его в `InputComponent` первой
системой Input-фазы. Поэтому команда, созданная в Input, доступна Simulation в
том же кадре. Presentation
читает результаты Simulation, но не создаёт gameplay-команды в ответ на
визуальное завершение. Bridge-событие, которое действительно должно влиять на
игру (например, контакт коллайдера), имеет отдельный явно описанный компонент и
обрабатывается в следующей разрешённой simulation-фазе.

Полный контракт срока жизни команд и переходов между текущими группами приведён
в [phase-transition-rules.md](phase-transition-rules.md). В нём также отмечены
legacy-нарушения, включая текущий путь `CheckHitSystem` → `HitCommand`.
Шаблон новой системы и checklist для review находятся в
[EcsSystemTemplate.cs.txt](EcsSystemTemplate.cs.txt) и
[pr-checklist.md](pr-checklist.md).

## Владение сессией и отложенными операциями

Каждая игровая сессия владеет своим `EcsWorld`, наборами систем, подписками,
view registry и cancellation token. При завершении сессии порядок teardown
следующий:

1. Остановить `Update`/`FixedUpdate` ticks и отменить внешние операции.
2. Убить или dispose все принадлежащие сессии tween и подписки.
3. Вызвать `EcsSystems.Destroy()`.
4. Освободить view и bridge-ресурсы.
5. Уничтожить `EcsWorld` и scoped DI-container.

Текущая точка владения — `GameEcsLoop` и созданный им `GameSession`.
`GameSession` создаёт `EcsWorld`/`EcsSystems`, публикует token отмены, запускает
системы и при dispose отменяет token до `EcsSystems.Destroy()` и
`EcsWorld.Destroy()`. `ApplicationState` реализует VContainer `ITickable`,
поэтому текущий общий ECS tick вызывается Unity `Update`, а не
`Observable.EveryUpdate()`. Разделение тиков на fixed/late-фазы остаётся
следующим шагом миграции.

При выходе `ApplicationState` сначала отписывается от restart event и отменяет
незавершённую загрузку scoped-зависимостей. Затем `GameEcsLoop.Stop()` убирает
активную session из tick path; `GameSession` отменяет token, уничтожает системы
и мир. Только после этого `DependenciesContainer` dispose scoped DI-container
и освобождает Addressables handles. Legacy DOTween пока не зарегистрирован у
session owner — это отдельная задача presentation bridge.

DOTween callback обязан принадлежать конкретной сессии и перед записью в ECS
проверять, что сессия активна, а entity ещё валидна. Callback не может замыкать
мир, который переживёт teardown. Временный `GameObject` и каждый tween имеют
одного явного владельца: entity view либо session owner.

## Команды, события и cleanup

Компонент-команда или событие определяет:

- создателя и фазу создания;
- потребителя или потребителей;
- срок жизни;
- систему, которая его удаляет.

По умолчанию команда живёт один кадр и удаляется `EndOfFrameCleanupSystem`.
Исключение документируется рядом с типом: например, многофазный `DeadCommand`
может существовать до того, как presentation безопасно освободит view.
Ни одна система не удаляет чужие одноразовые команды неявно.

## Пример: подбор монеты

1. Physics bridge фиксирует перекрытие `Collider2D` и публикует
   `CoinCollected` для entity монеты.
2. Simulation увеличивает счётчик игрока и помечает монету к удалению.
3. Presentation bridge запускает DOTween-анимацию только для view монеты;
   tween зарегистрирован у текущей сессии.
4. Cleanup удаляет одноразовое событие. После завершения анимации bridge
   освобождает view, а удаление entity выполняется по явно определённому
   правилу, не из DOTween callback в уничтоженный мир.

Таким образом результат подбора — изменение компонентов — не зависит от того,
успела ли анимация начаться, закончиться или была отменена рестартом.

## Правило для новой механики

Перед добавлением механики разработчик определяет её фазу, входные и выходные
компоненты, владельца view/асинхронной операции, срок жизни команд и порядок
очистки. Если механика требует Unity API в gameplay-системе, её следует
разделить на simulation-правило и bridge-адаптер.

Эта модель является целевой: существующие гибридные системы мигрируются к ней
постепенно, без одновременной переработки всех механик.
