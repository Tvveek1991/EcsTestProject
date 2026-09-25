using Leopotam.EcsLite;
using NUnit.Framework;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Systems;
using UnityEngine;

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
            session.Tick();

            Assert.That(probeSystem.InitCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.RunCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.DestroyCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.CancellationWasRequestedDuringDestroy, Is.True);
            Assert.That(probeSystem.WorldWasAliveDuringDestroy, Is.True);
        }

        [Test]
        public void BoxHitFlow_ChecksHitBeforeChangingHealthAndPresentation()
        {
            int checkHitIndex = GameSystemsComposer.GetOrderIndex(typeof(CheckHitSystem));
            int healthChangeIndex = GameSystemsComposer.GetOrderIndex(typeof(HealthChangeSystem));
            int healthViewChangeIndex = GameSystemsComposer.GetOrderIndex(typeof(HealthViewChangeSystem));

            Assert.That(checkHitIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(checkHitIndex, Is.LessThan(healthChangeIndex));
            Assert.That(healthChangeIndex, Is.LessThan(healthViewChangeIndex));
        }

        [Test]
        public void BoxHitCommand_DecreasesHealth()
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            var personData = ScriptableObject.CreateInstance<PersonData>();
            systems.Add(new HealthChangeSystem(personData));

            try
            {
                systems.Init();

                int boxEntity = world.NewEntity();
                world.GetPool<Health>().Add(boxEntity).Count = 100;
                world.GetPool<HitCommand>().Add(boxEntity).HitValue = 10;

                systems.Run();

                Assert.That(world.GetPool<Health>().Get(boxEntity).Count, Is.EqualTo(90));
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
                Object.DestroyImmediate(personData);
            }
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
