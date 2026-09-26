# Unity CI

Workflow [unity-editmode.yml](../../.github/workflows/unity-editmode.yml) runs
on pushes to `master` and `dev`, pull requests and manual dispatch. It starts
Unity `6000.3.19f1`, so loading the project compiles game assemblies before the
EditMode suite is executed.

The workflow uses `game-ci/unity-test-runner@v4` with `testMode: EditMode`.
The current CI gate is compilation and regression tests; coverage reports are
not uploaded as CI artifacts.

Before the first run, configure one of the following repository-secret sets:

- `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` — an activated Unity
  license file and account credentials for a Personal license;
- `UNITY_EMAIL`, `UNITY_PASSWORD`, `UNITY_SERIAL` — credentials for a
  Professional license.

The workflow checks for one of these sets before starting the Unity container,
so a missing activation is reported explicitly. Do not commit license files or
credentials. After every run, download the `unity-editmode-results` artifact
from GitHub Actions to inspect NUnit/XML results.

License activation and test-runner options follow the
[GameCI documentation](https://game.ci/docs/github/test-runner/).
