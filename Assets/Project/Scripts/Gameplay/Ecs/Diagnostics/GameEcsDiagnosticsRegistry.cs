namespace Project.Scripts.Gameplay.Ecs.Diagnostics
{
    public static class GameEcsDiagnosticsRegistry
    {
        private static GameEcsDiagnostics s_activeDiagnostics;

        public static GameEcsDiagnosticsSnapshot ActiveSnapshot => s_activeDiagnostics?.Snapshot;

        internal static void Activate(GameEcsDiagnostics diagnostics) =>
            s_activeDiagnostics = diagnostics;

        internal static void Deactivate(GameEcsDiagnostics diagnostics)
        {
            if (s_activeDiagnostics == diagnostics)
                s_activeDiagnostics = null;
        }
    }
}
