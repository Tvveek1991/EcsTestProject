## Project Overview

BestWoodApp is a Unity 2D project built with **Unity 6000.3.19f1**.

- Product name: `BestWoodApp`
- Product version: `1.0`
- Enabled build scene: `Assets/sablegames/scenes/Main.unity`
- Android: minimum SDK `25`; target SDK uses the Unity default.
- The Android and iOS application identifiers have not been configured yet.

## Project-owned code and assets

`Assets/sablegames/` is the root for all project-owned mechanics and application logic. Create and maintain the game's own runtime code, editor tooling, and project-specific helpers beneath this folder, using the `sablegames.*` namespace convention.

Do not place third-party plugins, SDKs, package code, or imported external helpers in `Assets/sablegames/`. Keep those in their original plugin/package locations (for example, `Assets/Plugins/` or `Packages/`) and change them only when the task explicitly requires it.

The project includes DOTween and a reusable sequence-tween module. Do not assume an existing gameplay architecture, dependency-injection setup, ECS implementation, Addressables configuration, or other SDKs beyond those files.

The existing tween module under `Assets/sablegames/modules/Tweens/` currently uses the `pingvigames.modules.tweens` namespace. Preserve that namespace when editing this imported module; use the `sablegames.*` convention for newly created project-owned code.

## Current layout

- `Assets/sablegames/scenes/Main.unity` — the enabled entry scene.
- `Assets/sablegames/modules/Tweens/` — sequence-tween component, editor inspector, and presets.
- `Assets/sablegames/scripts/utils/` — project utility extensions.
- `Assets/Plugins/Demigiant/DOTween/` — third-party DOTween plugin.
- `Assets/Resources/DOTweenSettings.asset` — DOTween configuration.
- `Packages/manifest.json` and `Packages/packages-lock.json` — declared and resolved Unity Package Manager dependencies.
- `ProjectSettings/` — Unity project configuration.

## Working in this Unity project

- Verify script edits in a running Unity Editor: let Unity refresh the assets, then inspect the Unity Console for compile errors.
- There is no CLI build, lint, or automated-test workflow configured. Use Unity Build Settings for local builds.
- Game code compiles into `Assembly-CSharp`; do not hand-edit Unity-generated `.sln` or `.csproj` files.
- Every Unity asset and script needs a matching `.meta` file. Prefer Unity Editor tooling when creating or deleting assets so metadata remains correct.
- Exclude generated/binary folders from searches: `Library/`, `Temp/`, `obj/`, `Logs/`, `BuildReports/`, and `AssetBundles/`.

## Code style

- Before editing C#, read `.claude/CODESTYLE.md` when it is available.
- Put new project-owned C# code under `Assets/sablegames/` and use `sablegames.*` namespaces.
- Follow the existing style in the relevant code. If no local convention exists, use clear names and keep changes narrow.
- Cleanly subscribe and unsubscribe events, and manage Unity object lifecycles explicitly.

## Dependencies and documentation

- `README.md` records the current project layout and direct Unity Package Manager dependencies. Update it whenever dependencies or their versions change, using `Packages/manifest.json` and `Packages/packages-lock.json` as the source of truth.
- Do not describe a package or SDK as installed until it appears in the project.

## Git

- Git remote: github.com. Work branches are ticket numbers (for example, `#5852`); typical flow is ticket branch → `dev` → `master` through pull requests.
- Commit messages reference their ticket, for example `last for #5893`.
