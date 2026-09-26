# ECS diagnostics

В Unity Editor откройте **Tools → BestWood → ECS Diagnostics** и перейдите в
Play Mode. Окно показывает snapshot активной игровой сессии после каждого
ECS-tick.

- **Key entities** — количество сущностей с ключевыми компонентами.
- **One-frame commands before cleanup** — команды, захваченные прямо перед
  `EndOfFrameCleanupSystem`; поэтому они видны даже после удаления в том же
  кадре.
- **Active phase/system** — последняя реально выполненная ECS-система и её
  декларативная фаза.
- **Presentation operations** — число активных tween в session-owned
  `GameplayTweenRegistry`.

Окно read-only: оно не создаёт сущности, не меняет компоненты и не влияет на
порядок систем.
