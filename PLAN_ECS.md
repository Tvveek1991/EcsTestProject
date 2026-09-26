# План развития ECS

## Назначение

Этот документ описывает безопасное развитие существующей ECS-архитектуры игры. Цель — сделать добавление механик, отладку и перезапуск игровых сессий предсказуемыми, сохранив LeoEcsLite и текущую интеграцию Unity.

Это **не** план миграции на Unity DOTS. Для текущей небольшой 2D-игры такая миграция даст высокий риск и стоимость без пропорциональной пользы. Основой остаётся LeoEcsLite; Unity `GameObject`, UI, Animator, `Rigidbody2D` и DOTween рассматриваются как presentation / bridge слой.

## Текущее состояние

Статический аудит выполнен 25 сентября 2026 года.

| Область | Наблюдение |
| --- | --- |
| ECS | LeoEcsLite, DI через VContainer; есть также пакет `ecslite-di` и editor debugger. |
| Масштаб | 144 C#-файла проекта, 50 ECS-систем, 35 компонентов, 7 view-классов. |
| Игровой цикл | `EcsWorld` и `EcsSystems` создаются в `ApplicationState`; вызов `Run()` происходит через `Observable.EveryUpdate()`. |
| Порядок | Последовательность систем задаётся порядком регистраций `IEcsSystem` в `GameSystemsInstaller`. Явных фаз кадра нет. |
| Физика | `Rigidbody2D` изменяется из общего кадра; отдельной fixed-фазы нет. |
| Ввод | Установлен Input System, однако геймплей читает legacy `UnityEngine.Input`; в Project Settings включены оба обработчика ввода. |
| Жизненный цикл | View хранятся в сервисах с ключом `int entity`; часть асинхронных tween/callback может пережить ECS-мир. |
| Тесты | Пользовательских автотестов не найдено. |

### Файлы, определяющие текущую архитектуру

- `Assets/Project/Scripts/Application/StateMachine/States/ApplicationState.cs` — запуск, обновление и уничтожение ECS-мира.
- `Assets/Project/Scripts/Gameplay/GameSystemsInstaller.cs` — регистрация и фактический порядок ECS-систем.
- `Assets/Project/Scripts/Gameplay/GamePlayInstaller.cs` — асинхронная загрузка ресурсов и регистрация Unity-зависимостей.
- `Assets/Project/Scripts/Gameplay/Systems/` — игровые системы.
- `Assets/Project/Scripts/Gameplay/Components/` — компоненты и команды.

## Проблемы и приоритеты

### P0 — безопасность жизненного цикла

1. `CoinsViewAnimationSystem` создаёт DOTween callback, использующий `EcsWorld`, а cleanup последовательностей в `Destroy()` сейчас отключён. При рестарте callback способен обратиться к уже уничтоженному миру.
2. `ApplicationState.Enter()` и `Sensor.OnTriggerExit2D()` используют `async void`. Исключения и отмена жизненного цикла не контролируются вызывающей стороной.
3. `Sensor` хранит один флаг контакта. Задержанный `OnTriggerExit2D` не учитывает несколько коллайдеров и повторный вход в триггер.

### P1 — предсказуемость ECS

1. Все системы исполняются в одной фазе `EveryUpdate`, включая логику, работающую с `Rigidbody2D`.
2. Порядок систем зависит от порядка VContainer-регистраций, который легко нарушить при добавлении нового класса.
3. Одноразовые компоненты-команды (`HitCommand`, `HealCommand`, `CoinsCounterChange`, `ReactionComponent`) очищаются разными системами и в разных местах.
4. `RunSystem` хранит `m_delayToIdle` на уровне системы, поэтому таймер разделяется всеми сущностями с `Run`.
5. [x] Восстановлен сценарий «атака ящика уменьшает здоровье»: `CheckHitSystem` временно перенесён в Simulation между `AttackSystem` и `HealthChangeSystem`; порядок закреплён EditMode-регрессионным тестом до переноса physics-группы в `FixedUpdate`.

### P2 — стоимость кадра и связность

1. `HealthViewFollowSystem` перебирает health view вместе с каждой сущностью с `Health` — стоимость растёт как произведение коллекций.
2. `CheckHitSystem` после `Physics2D.Raycast` перебирает все интерактивные объекты, чтобы найти сущность по `GameObject`.
3. В горячих системах используются LINQ-вызовы `Any` и `All` для сенсоров.
4. Логика игры напрямую знает о `Transform`, `Animator`, `SpriteRenderer`, `Rigidbody2D` и словарях view. Это приемлемый hybrid ECS, но граница должна быть единообразной и явной.

### P3 — опыт разработчика

1. Нет тестовой assembly и сценариев регрессии ECS.
2. Нет единого соглашения о фазах, командах, владении view и правилах производительности.
3. Инструкции в `AGENTS.md` задают `Assets/sablegames` и `sablegames.*` для нового кода, тогда как текущая игра расположена в `Assets/Project` и использует `Project.Scripts.*` / `Gameplay`. Перед новыми модулями это расхождение необходимо разрешить отдельным архитектурным решением.

## Целевая модель

### Разделение кадра

```text
Initialization (однократно)
        |
Input -> Simulation -> Physics -> Presentation -> Cleanup
                  \                         /
                   команды и события кадра
```

| Фаза | Ответственность | Примеры |
| --- | --- | --- |
| Initialization | Создание игровых сущностей и привязка view. | Уровень, игрок, UI. |
| Input | Сбор ввода в snapshot/команды. | Move, Jump, Attack. |
| Simulation | Чистые правила игры и изменение компонентов. | Здоровье, состояния, награды. |
| Physics | Чтение/запись Unity 2D physics на fixed tick. | Скорость `Rigidbody2D`, проверки земли. |
| Presentation | Отображение состояния. | Animator, камера, HUD, DOTween. |
| Cleanup | Удаление временных команд, view и сущностей. | `HitCommand`, уничтожение объекта. |

### Правила слоёв

- **Components** хранят игровое состояние; компоненты-ссылки на Unity имеют понятное имя `*ViewRef`, `*Binding` или `*Bridge`.
- **Simulation** не создаёт `GameObject`, не вызывает `Object.Instantiate`, `Destroy`, DOTween и не ищет Unity-компоненты.
- **Bridge/Presentation** сопоставляет ECS-сущности и Unity view, выполняет анимацию и уничтожает view.
- **Команды и события** живут ровно один кадр или до явно описанного потребителя; их удаляет централизованный cleanup.
- Каждая отложенная операция принадлежит активной игровой сессии и отменяется до уничтожения мира.

## Этапы реализации

### Этап 0. Базовая линия и соглашения

**Задачи**

1. [x] Canonical location и namespace согласованы: новый ECS-код находится в `Assets/Project/Scripts/Gameplay/Ecs/` и использует `Project.Scripts.Gameplay.Ecs.*`; `AGENTS.md` актуализирован под `Assets/Project`.
2. [x] Добавлен `Documentation/ECS/architecture.md` с целевой моделью, правилами систем и примером новой механики.
3. [x] Создана EditMode assembly `Project.Scripts.Gameplay.Ecs.Tests` для pure ECS-тестов и smoke-тест жизненного цикла `EcsWorld`; тест подтверждён Unity Test Runner (1/1 passed).
4. [x] Подготовлен профилировочный сценарий в `Documentation/ECS/profiling-baseline.md`: старт уровня, бег/прыжок, атака объектов, сбор монет и рестарт во время анимации.
5. [x] Зафиксирован baseline в `Documentation/ECS/baseline-results.md`: три прогона idle, movement, combat, coins и restart-during-tween с CPU/frame, GC Alloc/frame, memory и ручным временем рестарта; entities и active tween отмечены `N/A` до добавления diagnostics.
6. [x] Git-зависимости ECS, UniTask и VContainer закреплены на commit SHA в `Packages/manifest.json`; согласованные записи сохранены в `packages-lock.json`.

**Критерии готовности**

- Новичок может создать компонент, систему и временную команду по документированному шаблону.
- Есть воспроизводимые метрики для сравнения оптимизаций.
- Есть хотя бы smoke-тест создания/запуска/очистки ECS-мира.

### Этап 1. Явный игровой цикл и группы систем

**Задачи**

1. [x] Введены `GameEcsLoop` / `GameSession`, владеющие `EcsWorld`, системами и cancellation token сессии; порядок teardown проверяется EditMode-тестом.
2. [x] `Observable.EveryUpdate()` заменён на VContainer `ITickable`: `ApplicationState` получает Unity `Update` и тикает активный `GameEcsLoop`.
3. [x] Регистрация разделена на явные группы `Initialization`, `Input`, `Simulation`, `Physics`, `Presentation` и `Cleanup` в `GameSystemsInstaller`; пока они исполняются одним `Update` tick.
4. [x] Порядок систем вынесен из неявного `IEnumerable<IEcsSystem>` в декларативный `GameSystemsComposer`, который валидирует состав scoped-сессии.
5. [x] Правила перехода, сроков жизни команд и bridge-событий описаны в `Documentation/ECS/phase-transition-rules.md`; legacy-нарушения перечислены отдельно.
6. [x] Teardown упорядочен: `ApplicationState` отменяет загрузку и event-подписки, `GameEcsLoop.Stop()` убирает session из tick path, `GameSession` отменяет token и уничтожает systems/world, затем dispose scoped DI и Addressables handles.

**Критерии готовности**

- Порядок и фаза каждой системы видны в одном файле.
- Запись в `Rigidbody2D` выполняется только в fixed-фазе.
- Повторный рестарт не создаёт второй tick loop, не оставляет подписок и не вызывает обращений к уничтоженному миру.

### Этап 2. Надёжная связь ECS и Unity

**Задачи**

1. [x] Введены `EntityView` / `EntityLink` и scoped registry с проверкой entity, типа view и идентификатора текущей сессии.
2. [x] Gameplay-системы переведены с прямых dictionary view на typed `IEntityViewRegistry`; прежние специализированные view-сервисы удалены.
3. [x] `IGameplayViewFactory` создаёт prefab views, а finish/tutorial UI, датчики и эффекты создаются отдельными bridge-фабриками; в ECS-системах больше нет `Instantiate`.
4. [x] Все gameplay DOTween sequence/tween привязаны к session-owned registry; teardown гарантированно вызывает `Kill`, а callbacks, пишущие в ECS, защищены `TryExecute`.
5. [x] `Sensor` хранит множество пересекающихся коллайдеров, отменяет pending exit при новом enter и освобождает операции при уничтожении view.
6. [x] Игровой ввод переведён на `InputAction` asset: Unity слой обновляет один input snapshot, ECS генерирует из него команды.

**Критерии готовности**

- Рестарт во время сбора монеты, смерти и загрузки не даёт ошибок в Console.
- Удалённая сущность не может быть изменена отложенным callback.
- Два коллайдера на датчике корректно поддерживают состояние контакта.

### Этап 3. Устранение hot path и корректность многосущностной игры

**Задачи**

1. [x] `Collider2D` связан с entity через bridge/registry; после Raycast цель находится за O(1), без обхода всех объектов.
2. [x] Обновление health bar итерирует владельцев `Health` и обращается к их конкретному health view, без вложенного обхода коллекций.
3. Убрать LINQ из систем, запускаемых каждый кадр; заменить `Any`/`All` простыми циклами, когда профилировщик подтверждает путь.
4. [x] `Run` idle timer перенесён из поля системы в компонент состояния каждой сущности.
5. [x] Введён единый `EndOfFrameCleanupSystem` для однокадровых команд; исключение для многофазного `DeadCommand` задокументировано.
6. [x] Создание и уничтожение coin, particles и sensors профилировано в трёх прогонах; pooling не добавлен: coin и sensors создаются при старте сессии, а два runtime `Instantiate` соответствуют двум эффектам уничтожения ящиков и занимают менее 1.1 ms суммарно за прогон.

**Критерии готовности**

- Нет обходов «все view × все сущности» в кадре.
- Две игровые сущности одновременно могут независимо двигаться, терять состояние и умирать.
- В baseline-сценарии GC Alloc/frame не растёт из-за новых игровых систем.

### Этап 4. Инструменты и защита от регрессий

**Задачи**

1. [x] Добавлены EditMode-регрессии input → movement, hit → health → death, coin → counter и restart during tween; sensor overlap покрыт проверками множественных collider, повторного enter и уничтожения view. Все 26 тестов `Project.Scripts.Gameplay.Ecs.Tests` проходят в Unity Test Runner.
2. [x] Добавлено окно **Tools → BestWood → ECS Diagnostics**: read-only snapshot показывает ключевые сущности, одноразовые команды до cleanup, активную фазу/систему и число активных presentation tween. Использование описано в `Documentation/ECS/diagnostics.md`.
3. Добавить шаблон ECS-системы и чек-лист pull request:
   - указана фаза и зависимости по порядку;
   - фильтры и pools кэшируются в `Init`;
   - нет Unity-search/alloc в горячем `Run`;
   - команды очищаются;
   - view и async-операции имеют владельца;
   - добавлен тест для новой игровой логики.
4. Настроить CI как минимум на компиляцию проекта и запуск EditMode-тестов при наличии Unity runner в окружении.

**Критерии готовности**

- Новая механика проходит test + checklist до merge.
- В диагностике можно определить систему, создавшую зависшую команду или сущность.
- У команды есть короткое руководство по отладке ECS-сессии и рестарта.

## Рекомендуемый порядок поставки

1. Этап 0.
2. Этап 1, начиная с teardown и отмены подписок.
3. Этап 2, начиная с безопасного DOTween lifecycle и сенсоров.
4. Этап 3 для `HealthViewFollowSystem`, `CheckHitSystem` и `RunSystem`.
5. Этап 4 непрерывно: первые тесты и checklist добавляются уже с Этапа 0.

Не следует одновременно переписывать все компоненты и системы. Каждая миграция должна сохранять текущий игровой сценарий, проходить профилировочный smoke-test и проверяться в Unity Console на compile/runtime ошибки.

## Риски и меры

| Риск | Мера |
| --- | --- |
| Незаметно меняется порядок геймплейных эффектов | Мигрировать по одной фазе, зафиксировать ручной сценарий и тесты перед заменой. |
| Callback DOTween переживает мир | Сначала централизовать ownership/cancellation, только затем менять presentation-системы. |
| Миграция ввода меняет UX | Оставить legacy adapter на переходный период и сравнивать input snapshot в play mode. |
| Преждевременный pooling усложняет код | Вводить pooling только по данным Unity Profiler. |
| Смешение соглашений о путях/namespace | Зафиксировать решение на Этапе 0 до добавления новых модулей. |

## Определение готовой ECS-механики

Механика считается готовой, если:

1. Её система находится в определённой фазе и имеет явный порядок.
2. Gameplay-состояние не зависит от случайного порядка Unity callbacks.
3. Временные компоненты и view освобождаются при обычном завершении и при рестарте.
4. Отложенные операции отменяемы и не используют уничтоженный `EcsWorld`.
5. Есть тест либо документированный ручной сценарий, а для критичной логики — оба.
6. Она не ухудшает утверждённые метрики baseline без осознанного решения команды.
