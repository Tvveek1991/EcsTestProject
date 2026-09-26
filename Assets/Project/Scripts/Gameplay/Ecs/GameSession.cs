using System;
using System.Collections.Generic;
using System.Threading;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Ecs.Diagnostics;
using Project.Scripts.Gameplay.Services.TweenRegistry;

namespace Project.Scripts.Gameplay.Ecs
{
    public sealed class GameSession : IDisposable
    {
        private readonly CancellationTokenSource m_cancellationTokenSource;
        private readonly EcsWorld m_world;
        private readonly IEcsSystems m_updateSystems;
        private readonly IEcsSystems m_fixedSystems;
        private readonly List<IGameSessionOperation> m_sessionOperations;
        private readonly GameEcsDiagnostics m_diagnostics;

        private bool m_isStarted;
        private bool m_isDisposed;

        public GameSession(IEnumerable<IEcsSystem> systems, IEnumerable<IGameSessionOperation> sessionOperations = null)
        {
            if (systems == null)
                throw new ArgumentNullException(nameof(systems));

            m_cancellationTokenSource = new CancellationTokenSource();
            m_world = new EcsWorld();
            m_updateSystems = new EcsSystems(m_world);
            m_fixedSystems = new EcsSystems(m_world);
            m_sessionOperations = sessionOperations == null ? new List<IGameSessionOperation>() : new List<IGameSessionOperation>(sessionOperations);
            m_diagnostics = new GameEcsDiagnostics(FindTweenRegistry(m_sessionOperations));

            foreach (IEcsSystem system in systems)
            {
                if (system == null)
                    throw new ArgumentException("ECS session cannot contain a null system.", nameof(systems));

                var diagnosticsSystem = new EcsDiagnosticsSystemProxy(system, m_diagnostics);

                if (GameSystemsComposer.GetPhase(system.GetType()) == GameEcsPhase.Physics)
                    m_fixedSystems.Add(diagnosticsSystem);
                else
                    m_updateSystems.Add(diagnosticsSystem);
            }
        }

        public CancellationToken CancellationToken => m_cancellationTokenSource.Token;

        public bool IsRunning => m_isStarted && !m_isDisposed;

        public GameEcsDiagnosticsSnapshot Diagnostics => m_diagnostics.Snapshot;

        public void Start()
        {
            ThrowIfDisposed();

            if (m_isStarted)
                throw new InvalidOperationException("Game session is already started.");

            m_isStarted = true;
            m_diagnostics.Activate();

            try
            {
                m_updateSystems.Init();
                m_fixedSystems.Init();
                m_diagnostics.CaptureWorld(m_world);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Tick()
        {
            if (!IsRunning)
                return;

            m_diagnostics.BeginFrame();
            m_updateSystems.Run();
            m_diagnostics.CaptureWorld(m_world);
        }

        public void FixedTick()
        {
            if (!IsRunning)
                return;

            m_fixedSystems.Run();
            m_diagnostics.CaptureWorld(m_world);
        }

        public void Dispose()
        {
            if (m_isDisposed)
                return;

            m_isDisposed = true;
            m_cancellationTokenSource.Cancel();

            try
            {
                CancelSessionOperations();
            }
            finally
            {
                try
                {
                    DestroySystems();
                }
                finally
                {
                    m_world.Destroy();
                    m_diagnostics.Deactivate();
                    m_cancellationTokenSource.Dispose();
                }
            }
        }

        private void DestroySystems()
        {
            if (!m_isStarted)
                return;

            try
            {
                m_fixedSystems.Destroy();
            }
            finally
            {
                m_updateSystems.Destroy();
            }
        }

        private void CancelSessionOperations()
        {
            foreach (IGameSessionOperation sessionOperation in m_sessionOperations)
                sessionOperation?.Cancel();
        }

        private static IGameplayTweenRegistry FindTweenRegistry(IEnumerable<IGameSessionOperation> sessionOperations)
        {
            foreach (IGameSessionOperation sessionOperation in sessionOperations)
            {
                if (sessionOperation is IGameplayTweenRegistry tweenRegistry)
                    return tweenRegistry;
            }

            return null;
        }

        private void ThrowIfDisposed()
        {
            if (m_isDisposed)
                throw new ObjectDisposedException(nameof(GameSession));
        }
    }
}
