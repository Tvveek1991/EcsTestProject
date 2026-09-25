## Project Overview

BestWoodApp is a Unity 2D project built with **Unity 6000.3.19f1**.

- Product name: `BestWoodApp`
- Product version: `1.0`
- Enabled build scene: `Assets/Project/Scenes/Main.unity`
- Android: minimum SDK `25`; target SDK uses the Unity default.
- The Android and iOS application identifiers have not been configured yet.

## Project-owned code and assets

`Assets/Project/` is the root for project-owned scenes, prefabs, data, visuals
and application code. Create and maintain the game's own runtime code, editor
tooling and project-specific helpers beneath this folder.

New ECS gameplay code belongs in `Assets/Project/Scripts/Gameplay/` and uses the
`Project.Scripts.Gameplay.*` namespace convention. In particular, put new ECS
infrastructure under `Assets/Project/Scripts/Gameplay/Ecs/` and use
`Project.Scripts.Gameplay.Ecs.*`. Components, systems, presentation bridges and
tests may use explicit subnamespaces below this root.

The project has older namespaces such as `Gameplay`, `Application.*` and
`Project.Scripts.Gameplay.*`. Preserve a file's existing namespace when editing
it; do not perform broad namespace migrations as part of ECS work. New
gameplay/ECS code follows the canonical `Project.Scripts.Gameplay.*` convention
above.

Do not place third-party plugins, SDKs, package code or imported external helpers
in `Assets/Project/`. Keep them in their original plugin/package locations (for
example, `Assets/Plugins/` or `Packages/`) and change them only when the task
explicitly requires it.

The project currently uses LeoEcsLite, VContainer, UniTask, UniRx, Addressables,
the Unity Input System and DOTween. Inspect the existing integration before
adding or replacing a dependency.

## Current layout

- `Assets/Project/Scenes/Main.unity` — the enabled entry scene.
- `Assets/Project/Scripts/Application/` — bootstrap, state machine and application lifetime.
- `Assets/Project/Scripts/Gameplay/` — ECS components, systems, views, sensors, data and installers.
- `Assets/Project/Scripts/Loading/` and `Assets/Project/Scripts/AssetProvider/` — loading and asset integration.
- `Assets/Project/Prefabs/`, `Assets/Project/Data/` and `Assets/Project/Animations/` — project assets.
- `Assets/Plugins/Demigiant/DOTween/` — third-party DOTween plugin.
- `Assets/Resources/DOTweenSettings.asset` — DOTween configuration.
- `Packages/manifest.json` and `Packages/packages-lock.json` — declared and resolved Unity Package Manager dependencies.
- `ProjectSettings/` — Unity project configuration.
- `Documentation/ECS/architecture.md` — target ECS model; `PLAN_ECS.md` — its delivery plan.

## Working in this Unity project

- Verify script edits in a running Unity Editor: let Unity refresh the assets, then inspect the Unity Console for compile errors.
- There is no CLI build, lint, or automated-test workflow configured. Use Unity Build Settings for local builds.
- Game code compiles into assembly definitions such as `Gameplay`, `Application`, `Loading` and `AssetProvider`; do not hand-edit Unity-generated `.sln` or `.csproj` files.
- Every Unity asset and script needs a matching `.meta` file. Prefer Unity Editor tooling when creating or deleting assets so metadata remains correct.
- Exclude generated/binary folders from searches: `Library/`, `Temp/`, `obj/`, `Logs/`, `BuildReports/`, and `AssetBundles/`.

## Code style

- Before editing C#, read `.claude/CODESTYLE.md` when it is available.
- Put new gameplay/ECS C# code under `Assets/Project/Scripts/Gameplay/` and use `Project.Scripts.Gameplay.*` namespaces.
- Preserve an existing class's namespace and local conventions when modifying it; do not rename legacy namespaces solely for consistency.
- Follow the existing style in the relevant code. If no local convention exists, use clear names and keep changes narrow.
- Cleanly subscribe and unsubscribe events, and manage Unity object lifecycles explicitly.

## Dependencies and documentation

- Keep `PLAN_ECS.md` and relevant files in `Documentation/` current when an architectural decision or its delivery status changes.
- When dependencies or versions change, use `Packages/manifest.json` and `Packages/packages-lock.json` as the source of truth for documentation.
- Do not describe a package or SDK as installed until it appears in the project.

## Git

- Git remote: github.com. Work branches are ticket numbers (for example, `#5852`); typical flow is ticket branch → `dev` → `master` through pull requests.
- Commit messages reference their ticket, for example `last for #5893`.
