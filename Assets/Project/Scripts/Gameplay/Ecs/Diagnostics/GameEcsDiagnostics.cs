using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.TweenRegistry;

namespace Project.Scripts.Gameplay.Ecs.Diagnostics
{
    public sealed class GameEcsDiagnostics
    {
        private readonly IGameplayTweenRegistry m_tweenRegistry;

        public GameEcsDiagnostics(IGameplayTweenRegistry tweenRegistry)
        {
            m_tweenRegistry = tweenRegistry;
            Snapshot = new GameEcsDiagnosticsSnapshot();
        }

        public GameEcsDiagnosticsSnapshot Snapshot { get; }

        public void Activate()
        {
            Snapshot.IsSessionRunning = true;
            GameEcsDiagnosticsRegistry.Activate(this);
        }

        public void Deactivate()
        {
            Snapshot.IsSessionRunning = false;
            GameEcsDiagnosticsRegistry.Deactivate(this);
        }

        public void BeginFrame()
        {
            Snapshot.FrameIndex++;
            Snapshot.ActivePhase = GameEcsPhase.None;
            Snapshot.ActiveSystemName = null;
        }

        public void SetActiveSystem(GameEcsPhase phase, string systemName)
        {
            Snapshot.ActivePhase = phase;
            Snapshot.ActiveSystemName = systemName;
        }

        public void CaptureOneFrameCommands(EcsWorld world)
        {
            Snapshot.OneFrameHitCommandCount = Count<HitCommand>(world);
            Snapshot.OneFrameHealCommandCount = Count<HealCommand>(world);
            Snapshot.OneFrameJumpCommandCount = Count<Jump>(world);
            Snapshot.OneFrameReactionCount = Count<ReactionComponent>(world);
            Snapshot.OneFrameCoinsCounterChangeCount = Count<CoinsCounterChange>(world);
        }

        public void CaptureWorld(EcsWorld world)
        {
            Snapshot.PlayerCount = Count<Player>(world);
            Snapshot.HealthCount = Count<Health>(world);
            Snapshot.CoinViewCount = Count<CoinViewKeeper>(world);
            Snapshot.DeadCommandCount = Count<DeadCommand>(world);
            Snapshot.ActivePresentationOperationCount = m_tweenRegistry?.ActiveTweenCount ?? 0;
        }

        private static int Count<TComponent>(EcsWorld world) where TComponent : struct =>
            world.Filter<TComponent>().End().GetEntitiesCount();
    }
}
