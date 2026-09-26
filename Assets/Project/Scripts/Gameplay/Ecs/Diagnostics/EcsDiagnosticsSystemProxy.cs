using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Systems;

namespace Project.Scripts.Gameplay.Ecs.Diagnostics
{
    internal sealed class EcsDiagnosticsSystemProxy : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem, IEcsDestroySystem
    {
        private readonly IEcsSystem m_system;
        private readonly GameEcsDiagnostics m_diagnostics;
        private readonly GameEcsPhase m_phase;

        public EcsDiagnosticsSystemProxy(IEcsSystem system, GameEcsDiagnostics diagnostics)
        {
            m_system = system;
            m_diagnostics = diagnostics;
            m_phase = GameSystemsComposer.GetPhase(system.GetType());
        }

        public void PreInit(IEcsSystems systems)
        {
            if (m_system is IEcsPreInitSystem preInitSystem)
                preInitSystem.PreInit(systems);
        }

        public void Init(IEcsSystems systems)
        {
            if (m_system is not IEcsInitSystem initSystem)
                return;

            SetActiveSystem();
            initSystem.Init(systems);
        }

        public void Run(IEcsSystems systems)
        {
            if (m_system is not IEcsRunSystem runSystem)
                return;

            SetActiveSystem();

            if (m_system is EndOfFrameCleanupSystem)
                m_diagnostics.CaptureOneFrameCommands(systems.GetWorld());

            runSystem.Run(systems);
        }

        public void PostRun(IEcsSystems systems)
        {
            if (m_system is not IEcsPostRunSystem postRunSystem)
                return;

            postRunSystem.PostRun(systems);
        }

        public void Destroy(IEcsSystems systems)
        {
            if (m_system is not IEcsDestroySystem destroySystem)
                return;

            destroySystem.Destroy(systems);
        }

        private void SetActiveSystem()
        {
            if (m_phase != GameEcsPhase.None)
                m_diagnostics.SetActiveSystem(m_phase, m_system.GetType().Name);
        }
    }
}
