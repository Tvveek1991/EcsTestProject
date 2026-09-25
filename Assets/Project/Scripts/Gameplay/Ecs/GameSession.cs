using System;
using System.Collections.Generic;
using System.Threading;
using Leopotam.EcsLite;

namespace Project.Scripts.Gameplay.Ecs
{
    public sealed class GameSession : IDisposable
    {
        private readonly CancellationTokenSource m_cancellationTokenSource;
        private readonly EcsWorld m_world;
        private readonly IEcsSystems m_systems;
        private readonly List<IGameSessionOperation> m_sessionOperations;

        private bool m_isStarted;
        private bool m_isDisposed;

        public GameSession(IEnumerable<IEcsSystem> systems, IEnumerable<IGameSessionOperation> sessionOperations = null)
        {
            if (systems == null)
                throw new ArgumentNullException(nameof(systems));

            m_cancellationTokenSource = new CancellationTokenSource();
            m_world = new EcsWorld();
            m_systems = new EcsSystems(m_world);
            m_sessionOperations = sessionOperations == null ? new List<IGameSessionOperation>() : new List<IGameSessionOperation>(sessionOperations);

            foreach (IEcsSystem system in systems)
            {
                if (system == null)
                    throw new ArgumentException("ECS session cannot contain a null system.", nameof(systems));

                m_systems.Add(system);
            }
        }

        public CancellationToken CancellationToken => m_cancellationTokenSource.Token;

        public bool IsRunning => m_isStarted && !m_isDisposed;

        public void Start()
        {
            ThrowIfDisposed();

            if (m_isStarted)
                throw new InvalidOperationException("Game session is already started.");

            m_isStarted = true;

            try
            {
                m_systems.Init();
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

            m_systems.Run();
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
                    if (m_isStarted)
                        m_systems.Destroy();
                }
                finally
                {
                    m_world.Destroy();
                    m_cancellationTokenSource.Dispose();
                }
            }
        }

        private void CancelSessionOperations()
        {
            foreach (IGameSessionOperation sessionOperation in m_sessionOperations)
                sessionOperation?.Cancel();
        }

        private void ThrowIfDisposed()
        {
            if (m_isDisposed)
                throw new ObjectDisposedException(nameof(GameSession));
        }
    }
}
