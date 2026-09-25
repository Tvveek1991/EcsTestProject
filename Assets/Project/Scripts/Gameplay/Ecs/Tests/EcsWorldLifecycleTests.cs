using Leopotam.EcsLite;
using NUnit.Framework;

namespace Project.Scripts.Gameplay.Ecs.Tests
{
    public sealed class EcsWorldLifecycleTests
    {
        private EcsWorld m_world;

        private EcsSystems m_systems;

        private LifecycleProbeSystem m_probeSystem;

        private bool m_systemsDestroyed;

        private bool m_worldDestroyed;

        [SetUp]
        public void SetUp()
        {
            m_world = new EcsWorld();
            m_probeSystem = new LifecycleProbeSystem();
            m_systems = new EcsSystems(m_world);
            m_systems.Add(m_probeSystem);
        }

        [TearDown]
        public void TearDown()
        {
            if (!m_systemsDestroyed)
                m_systems.Destroy();

            if (!m_worldDestroyed && m_world.IsAlive())
                m_world.Destroy();
        }

        [Test]
        public void WorldLifecycle_InitializesRunsAndDestroysSystemsBeforeWorld()
        {
            m_systems.Init();
            m_systems.Run();
            m_systems.Destroy();
            m_systemsDestroyed = true;

            m_world.Destroy();
            m_worldDestroyed = true;

            Assert.That(m_probeSystem.InitCallCount, Is.EqualTo(1));
            Assert.That(m_probeSystem.RunCallCount, Is.EqualTo(1));
            Assert.That(m_probeSystem.DestroyCallCount, Is.EqualTo(1));
            Assert.That(m_probeSystem.WorldWasAliveDuringDestroy, Is.True);
            Assert.That(m_world.IsAlive(), Is.False);
        }

        [Test]
        public void GameSession_CancelsBeforeDestroyingSystemsAndWorld()
        {
            var probeSystem = new SessionLifecycleProbeSystem();
            using var session = new GameSession(new IEcsSystem[] { probeSystem });

            probeSystem.SetCancellationToken(session.CancellationToken);
            session.Start();
            session.Tick();
            session.Dispose();

            Assert.That(probeSystem.InitCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.RunCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.DestroyCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.CancellationWasRequestedDuringDestroy, Is.True);
            Assert.That(probeSystem.WorldWasAliveDuringDestroy, Is.True);
        }

        private sealed class LifecycleProbeSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
        {
            public int InitCallCount { get; private set; }

            public int RunCallCount { get; private set; }

            public int DestroyCallCount { get; private set; }

            public bool WorldWasAliveDuringDestroy { get; private set; }

            public void Init(IEcsSystems systems)
            {
                InitCallCount++;
            }

            public void Run(IEcsSystems systems)
            {
                RunCallCount++;
            }

            public void Destroy(IEcsSystems systems)
            {
                DestroyCallCount++;
                WorldWasAliveDuringDestroy = systems.GetWorld().IsAlive();
            }
        }

        private sealed class SessionLifecycleProbeSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
        {
            private System.Threading.CancellationToken m_cancellationToken;

            public int InitCallCount { get; private set; }

            public int RunCallCount { get; private set; }

            public int DestroyCallCount { get; private set; }

            public bool CancellationWasRequestedDuringDestroy { get; private set; }

            public bool WorldWasAliveDuringDestroy { get; private set; }

            public void SetCancellationToken(System.Threading.CancellationToken cancellationToken)
            {
                m_cancellationToken = cancellationToken;
            }

            public void Init(IEcsSystems systems)
            {
                InitCallCount++;
            }

            public void Run(IEcsSystems systems)
            {
                RunCallCount++;
            }

            public void Destroy(IEcsSystems systems)
            {
                DestroyCallCount++;
                CancellationWasRequestedDuringDestroy = m_cancellationToken.IsCancellationRequested;
                WorldWasAliveDuringDestroy = systems.GetWorld().IsAlive();
            }
        }
    }
}
