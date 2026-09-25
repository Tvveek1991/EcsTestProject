using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Project.Scripts.Gameplay.Ecs
{
    public sealed class GameEcsLoop : IDisposable
    {
        private GameSession m_session;

        public bool IsRunning => m_session != null && m_session.IsRunning;

        public void Start(IEnumerable<IEcsSystem> systems)
        {
            if (m_session != null)
                throw new InvalidOperationException("Game ECS loop is already running.");

            GameSession session = new GameSession(systems);

            try
            {
                session.Start();
                m_session = session;
            }
            catch
            {
                session.Dispose();
                throw;
            }
        }

        public void Tick() =>
            m_session?.Tick();

        public void Stop()
        {
            GameSession session = m_session;
            m_session = null;
            session?.Dispose();
        }

        public void Dispose() =>
            Stop();
    }
}
