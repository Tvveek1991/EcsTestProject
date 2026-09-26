# ECS pull request checklist

Используйте этот checklist для каждого PR, который добавляет или меняет ECS
компоненты, системы, presentation bridge либо lifecycle сессии.

## Фаза и порядок

- [ ] Система добавлена в `GameSystemsInstaller` и объявлена в
  `GameSystemsComposer` в корректной фазе.
- [ ] В описании PR указаны входные/выходные компоненты и причина порядка
  относительно соседних систем.
- [ ] Переход между фазами соответствует
  [phase-transition-rules.md](phase-transition-rules.md).

## Данные и hot path

- [ ] Фильтры и pools кэшируются в `Init`; в `Run` нет поиска компонентов по
  сцене, LINQ, скрытых allocation и работы с неограниченными коллекциями.
- [ ] Simulation не вызывает Unity API, `Instantiate`, `Destroy`, Animator,
  Rigidbody2D или DOTween.
- [ ] Изменение hot path основано на измерении Unity Profiler, а не на
  предположении.

## Команды, view и async

- [ ] Для каждой команды указан producer, consumer, срок жизни и owner
  cleanup. Однокадровые команды удаляет `EndOfFrameCleanupSystem`.
- [ ] Исключение из cleanup (например, `DeadCommand`) документировано рядом с
  компонентом и consumer.
- [ ] View связан с entity только через typed `IEntityViewRegistry`; collider
  регистрируется и снимается вместе с view.
- [ ] Tween, callback, UniTask и подписка принадлежат сессии, отменяются при
  teardown и не пишут в уничтоженный `EcsWorld`.

## Проверка

- [ ] Добавлен или обновлён EditMode-тест для новой игровой цепочки.
- [ ] После Unity asset refresh Console не содержит compile/runtime ошибок.
- [ ] Пройден релевантный ручной Play Mode сценарий; для изменения
  производительности приложены сопоставимые profiler-данные.
- [ ] При изменении архитектуры обновлены `PLAN_ECS.md` и документы в
  `Documentation/ECS/`.

Шаблон новой системы: [EcsSystemTemplate.cs.txt](EcsSystemTemplate.cs.txt).
