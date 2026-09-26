namespace Project.Scripts.Gameplay.Ecs.Diagnostics
{
    public sealed class GameEcsDiagnosticsSnapshot
    {
        public bool IsSessionRunning { get; internal set; }

        public int FrameIndex { get; internal set; }

        public GameEcsPhase ActivePhase { get; internal set; }

        public string ActiveSystemName { get; internal set; }

        public int PlayerCount { get; internal set; }

        public int HealthCount { get; internal set; }

        public int CoinViewCount { get; internal set; }

        public int DeadCommandCount { get; internal set; }

        public int OneFrameHitCommandCount { get; internal set; }

        public int OneFrameHealCommandCount { get; internal set; }

        public int OneFrameJumpCommandCount { get; internal set; }

        public int OneFrameReactionCount { get; internal set; }

        public int OneFrameCoinsCounterChangeCount { get; internal set; }

        public int ActivePresentationOperationCount { get; internal set; }
    }
}
