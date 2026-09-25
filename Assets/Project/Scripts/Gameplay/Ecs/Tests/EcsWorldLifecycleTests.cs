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
    }
}
