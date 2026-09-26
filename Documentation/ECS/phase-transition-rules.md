# Правила перехода между ECS-фазами

## Назначение

Этот документ задаёт контракт между группами `Initialization`, `Input`,
`Simulation`, `Physics`, `Presentation` и `Cleanup`. Он действует для всех
новых систем и описывает, какие нарушения уже есть в legacy-коде. Пока группы
исполняются одним Unity `Update`, но их относительный порядок уже
`Input → Simulation → Physics → Presentation → Cleanup`.

## Базовые правила

1. Компонент состояния живёт до явного удаления entity или до замены
   владельцем. Он не является сигналом кадра.
2. Команда или событие имеет одного владельца cleanup и документированных
   producer/consumer. Однокадровая команда удаляется только после того, как
   отработали все её разрешённые consumers.
3. Unity input bridge обновляет единый `GameplayInputSnapshot` из
   `GameplayInputActions` ровно один раз перед `GameEcsLoop.Tick()`.
   `InputSystem` копирует этот snapshot в `InputComponent`; системы
   `CheckInput*` могут создавать intent-компоненты, доступные Simulation в
   этом же кадре.
4. Simulation читает input intents, изменяет gameplay-state и создаёт
   намерения для Physics. Она не обращается к Unity view, UI, Animator,
   Rigidbody2D или DOTween. `CheckHitSystem` временно является legacy-
   исключением: он использует raycast, но исполняется в Simulation, чтобы
   `HitCommand` был обработан в том же кадре.
5. Physics читает намерения Simulation и Unity physics, а результат публикует
   как data/event для **следующего** Simulation tick. Physics не удаляет
   команду до её consumer.
6. Presentation читает gameplay-state и presentation requests. Она может
   изменить view, UI, Animator или tween, но не пишет в `EcsWorld` напрямую.
   Для обратного действия создаётся явное `*BridgeEvent`, которое мост
   добавляет в начале следующей Simulation-фазы.
7. Cleanup удаляет одноразовые команды, завершённые view и entities после
   consumers. Presentation не владеет очисткой gameplay-команд.

## Срок жизни текущих компонентов

| Тип | Producer | Consumer | Срок жизни и owner |
| --- | --- | --- | --- |
| `GameplayInputSnapshot` / `InputComponent` | Unity input bridge / `InputSystem` | `CheckInput*`, `EndGameSystem` | Bridge читает `InputAction` asset перед ECS tick, `InputSystem` один раз копирует snapshot; `InputComponent` удаляется вместе с session. |
| `Jump` | `CheckInputJumpSystem` | `JumpSystem` | Intent текущего кадра; удаляется `EndOfFrameCleanupSystem` после всех фаз. |
| `Run`, `Rolling`, `Block`, `Attack` | `CheckInput*` | соответствующие movement systems | Это краткоживущие gameplay-state, а не универсальные команды. Их владелец удаляет компонент по собственному condition. |
| `HitCommand` от `Q` | `CheckInputHurtSystem` | `HealthChangeSystem`, `HealthViewChangeSystem` | Доступен Simulation в тот же кадр; удаляется `EndOfFrameCleanupSystem` после presentation consumers. |
| `HitCommand` от raycast | `CheckHitSystem` (Simulation, временно) | `HealthChangeSystem`, `HealthViewChangeSystem` | Создаётся после `AttackSystem` и до `HealthChangeSystem`, обрабатывается в том же `Update` и удаляется end-of-frame. |
| `HealCommand` | future Simulation/bridge producer | `HealthChangeSystem`, `HealthViewChangeSystem` | Однокадровая команда; удаляется `EndOfFrameCleanupSystem` после presentation consumer. |
| `CoinsCounterChange` | bridge после завершения coin animation | `CoinsCounterChangeSystem` | Одноразовая Simulation entity; удаляется `EndOfFrameCleanupSystem`. Callback DOTween пока пишет её напрямую — legacy-нарушение. |
| `CoinViewFlyAwayAnimation` | `CoinsViewCheckSystem` | `CoinsViewAnimationSystem` | Presentation request. После старта анимации entity удаляется `CoinsViewAnimationSystem.PostRun`; callback не должен создавать gameplay-команды напрямую. |
| `DeadCommand` | `CheckDeathSystem` | destruction и death presentation systems | Многофазная команда со статусами `Ready → Started → Completed`; остаётся до перевода owner в `Dead`. |
| `ReactionComponent` | Finish UI bridge | `ReactionSystem` | Однокадровая Simulation-команда; удаляется `EndOfFrameCleanupSystem`, если restart не уничтожил session раньше. UI callback не должен вызывать `EcsWorld.NewEntity()` напрямую. |

## Действующие переходы и известные нарушения

| Переход | Разрешённое поведение | Текущее состояние |
| --- | --- | --- |
| Input → Simulation | Input intent создан в `CheckInput*` и читается Simulation в том же `Update`. | Используется для `HitCommand` от `Q`; допустимо до ввода отдельного input snapshot adapter. |
| Simulation → Physics | Simulation публикует `Jump`, `Run`, `Rolling`, `Block`, `Attack`; Physics применяет их к `Rigidbody2D`/raycast. | Группы уже разделены, но всё ещё работают в одном `Update`. |
| Simulation → Simulation | Временный legacy-adapter может опубликовать команду для следующей системы той же фазы. | `CheckHitSystem` работает после `AttackSystem` и до `HealthChangeSystem`; регрессионный тест закрепляет этот порядок до переноса physics-группы в `FixedUpdate`. |
| Physics → Simulation (next tick) | Результат physics живёт до следующей Simulation-фазы. | Пока нет systems, публикующих physics-результат. При их добавлении запрещено очищать результат в Presentation до consumer следующего tick. |
| Presentation → Simulation (next tick) | UI/tween публикует typed `*BridgeEvent`, а bridge добавляет gameplay-команду в начале следующей Simulation. | **Нарушено:** Finish UI создаёт `ReactionComponent`, а coin DOTween callback создаёт `CoinsCounterChange` прямо через `EcsWorld`. |
| Cleanup → session teardown | Сначала останавливаются ticks и отменяются внешние операции; затем systems, world и scope. | `GameSession` уже отменяет token перед `EcsSystems.Destroy()` и `EcsWorld.Destroy()`; ownership tween ещё предстоит внедрить. |

## Требования к новой механике

Перед добавлением системы разработчик указывает в её комментарии или
документации фазу, входные/выходные компоненты, owner cleanup и допустимый
переход. Если операция начинается в UI, callback или DOTween, она обязана
создать bridge-event, привязанный к `GameSession.CancellationToken`; callback
проверяет актуальность session и не замыкает `EcsWorld`.

`EndOfFrameCleanupSystem` — единственный owner очистки `HitCommand`,
`HealCommand`, `ReactionComponent`, `Jump` и `CoinsCounterChange`. Его нельзя
использовать для `DeadCommand`: эта команда многофазно переходит
`Ready → Started → Completed` и удаляется `CheckDeathSystem` только при
переводе owner в `Dead`. Другие исключения должны быть перечислены в таблице.
