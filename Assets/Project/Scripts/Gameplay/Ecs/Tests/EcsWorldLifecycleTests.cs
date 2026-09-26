using Leopotam.EcsLite;
using NUnit.Framework;
using System.Collections;
using System.Reflection;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Services.Input;
using Project.Scripts.Gameplay.Services.BridgeFactory;
using Project.Scripts.Gameplay.Services.CameraService;
using Project.Scripts.Gameplay.Services.TweenRegistry;
using Project.Scripts.Gameplay.Services.ViewFactory;
using Project.Scripts.Gameplay.Sensors;
using Project.Scripts.Gameplay.Systems;
using Project.Scripts.Gameplay.Systems.Input;
using Project.Scripts.Gameplay.Views;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

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
            var sessionOperation = new SessionOperationProbe();
            using var session = new GameSession(new IEcsSystem[] { probeSystem }, new IGameSessionOperation[] { sessionOperation });

            probeSystem.SetCancellationToken(session.CancellationToken);
            probeSystem.SetSessionOperation(sessionOperation);
            session.Start();
            session.Tick();
            session.Dispose();
            session.Tick();

            Assert.That(probeSystem.InitCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.RunCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.DestroyCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.CancellationWasRequestedDuringDestroy, Is.True);
            Assert.That(probeSystem.SessionOperationWasCanceledDuringDestroy, Is.True);
            Assert.That(probeSystem.WorldWasAliveDuringDestroy, Is.True);
            Assert.That(sessionOperation.CancelCallCount, Is.EqualTo(1));
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

        [Test]
        public void GameplayTweenRegistry_BlocksCallbacksAfterCancellation()
        {
            var tweenRegistry = new GameplayTweenRegistry();
            bool callbackWasCalled = false;

            tweenRegistry.Cancel();

            Assert.That(tweenRegistry.IsSessionActive, Is.False);
            Assert.That(tweenRegistry.TryExecute(() => callbackWasCalled = true), Is.False);
            Assert.That(callbackWasCalled, Is.False);
        }

        [Test]
        public void InputSystem_CopiesSnapshotBeforeInputCommandsRun()
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            var inputReader = new InputReaderProbe
            {
                Snapshot = new GameplayInputSnapshot
                {
                    IsEnabled = true,
                    IsJump = true,
                    IsRolling = true,
                    IsMoveRight = true,
                    IsAttack = true,
                    IsBlock = true
                }
            };
            systems.Add(new InputSystem(inputReader));

            try
            {
                systems.Init();
                systems.Run();

                var inputFilter = world.Filter<InputComponent>().End();
                int inputEntity = inputFilter.GetRawEntities()[0];
                InputComponent input = world.GetPool<InputComponent>().Get(inputEntity);

                Assert.That(input.IsEnabled, Is.True);
                Assert.That(input.IsJump, Is.True);
                Assert.That(input.IsRolling, Is.True);
                Assert.That(input.IsMoveRight, Is.True);
                Assert.That(input.IsAttack, Is.True);
                Assert.That(input.IsBlock, Is.True);
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
            }
        }

        [Test]
        public void GameSession_DestroysSystemsWhenSessionOperationCancellationFails()
        {
            var probeSystem = new LifecycleProbeSystem();
            var session = new GameSession(new IEcsSystem[] { probeSystem }, new IGameSessionOperation[] { new ThrowingSessionOperation() });
            session.Start();

            Assert.Throws<System.InvalidOperationException>(() => session.Dispose());
            Assert.That(probeSystem.DestroyCallCount, Is.EqualTo(1));
            Assert.That(probeSystem.WorldWasAliveDuringDestroy, Is.True);
        }

        [Test]
        public void EntityViewRegistry_RegistersAndUnregistersLinkedView()
        {
            var gameObject = new GameObject("Registry test view");
            var registry = new EntityViewRegistry();
            var personView = gameObject.AddComponent<PersonView>();

            try
            {
                registry.Register(1, personView);

                Assert.That(registry.TryGet(1, out PersonView registeredView), Is.True);
                Assert.That(registeredView, Is.SameAs(personView));
                Assert.That(registry.TryGetEntity(personView, out int entity), Is.True);
                Assert.That(entity, Is.EqualTo(1));

                Assert.That(registry.Unregister(1), Is.True);
                Assert.That(registry.TryGet(1, out PersonView _), Is.False);
                Assert.That(personView.Link.IsLinked, Is.False);
            }
            finally
            {
                registry.Dispose();
                Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void EntityViewRegistry_DoesNotReuseViewFromAnotherSession()
        {
            var gameObject = new GameObject("Session registry test view");
            var firstRegistry = new EntityViewRegistry();
            var secondRegistry = new EntityViewRegistry();
            var personView = gameObject.AddComponent<PersonView>();

            try
            {
                firstRegistry.Register(1, personView);

                Assert.Throws<System.InvalidOperationException>(() => secondRegistry.Register(1, personView));

                firstRegistry.Clear();
                secondRegistry.Register(1, personView);

                Assert.That(secondRegistry.TryGet(1, out PersonView registeredView), Is.True);
                Assert.That(registeredView, Is.SameAs(personView));
            }
            finally
            {
                firstRegistry.Dispose();
                secondRegistry.Dispose();
                Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void EntityViewRegistry_MapsAllViewCollidersAndCleansThemOnUnregister()
        {
            var root = new GameObject("Collider registry view");
            var child = new GameObject("Child collider");
            var registry = new EntityViewRegistry();
            var view = root.AddComponent<ObjectView>();
            var rootCollider = root.AddComponent<BoxCollider2D>();
            child.transform.SetParent(root.transform);
            var childCollider = child.AddComponent<CircleCollider2D>();

            try
            {
                registry.Register(5, view);
                registry.RegisterCollider(5, rootCollider);
                registry.RegisterCollider(5, childCollider);

                Assert.That(registry.TryGetEntity(rootCollider, out int rootEntity), Is.True);
                Assert.That(rootEntity, Is.EqualTo(5));
                Assert.That(registry.TryGetEntity(childCollider, out int childEntity), Is.True);
                Assert.That(childEntity, Is.EqualTo(5));

                registry.Unregister(5);

                Assert.That(registry.TryGetEntity(rootCollider, out _), Is.False);
                Assert.That(registry.TryGetEntity(childCollider, out _), Is.False);
            }
            finally
            {
                registry.Dispose();
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CheckHitSystem_ResolvesRaycastColliderThroughRegistry()
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            var registry = new EntityViewRegistry();
            var playerObject = new GameObject("Attacking player");
            var checkerObject = new GameObject("Attack checker");
            var targetObject = new GameObject("Raycast target");
            var playerView = playerObject.AddComponent<PersonView>();
            var playerRenderer = playerObject.AddComponent<SpriteRenderer>();
            var targetView = targetObject.AddComponent<ObjectView>();
            var targetCollider = targetObject.AddComponent<BoxCollider2D>();

            checkerObject.transform.SetParent(playerObject.transform);
            checkerObject.transform.position = Vector3.zero;
            targetObject.transform.position = Vector3.right;
            targetObject.layer = LayerMask.NameToLayer("InteractiveObject");
            typeof(PersonView).GetField("m_checkerSpawnPoint", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(playerView, checkerObject.transform);

            systems.Add(new CheckHitSystem(registry));

            try
            {
                Assert.That(targetObject.layer, Is.GreaterThanOrEqualTo(0));

                int playerEntity = world.NewEntity();
                world.GetPool<PersonViewComponent>().Add(playerEntity);
                world.GetPool<Attack>().Add(playerEntity).IsActive = true;
                world.GetPool<SpriteRendererKeeper>().Add(playerEntity).SpriteRenderer = playerRenderer;
                registry.Register(playerEntity, playerView);

                int targetEntity = world.NewEntity();
                world.GetPool<Health>().Add(targetEntity).Count = 100;
                registry.Register(targetEntity, targetView);
                registry.RegisterCollider(targetEntity, targetCollider);

                systems.Init();
                Physics2D.SyncTransforms();
                systems.Run();

                Assert.That(world.GetPool<HitCommand>().Has(targetEntity), Is.True);
                Assert.That(world.GetPool<HitCommand>().Get(targetEntity).HitValue, Is.EqualTo(10));
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
                registry.Dispose();
                Object.DestroyImmediate(playerObject);
                Object.DestroyImmediate(targetObject);
            }
        }

        [Test]
        public void HealthViewFollowSystem_FollowsEachHealthOwnerViewDirectly()
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            var registry = new EntityViewRegistry();
            var cameraObject = new GameObject("Health camera");
            var personObject = new GameObject("Health person");
            var objectObject = new GameObject("Health object");
            var personPointObject = new GameObject("Person health point");
            var objectPointObject = new GameObject("Object health point");
            var personHealthObject = new GameObject("Person health view");
            var objectHealthObject = new GameObject("Object health view");
            var camera = cameraObject.AddComponent<Camera>();
            var personView = personObject.AddComponent<PersonView>();
            var objectView = objectObject.AddComponent<ObjectView>();
            var personHealthView = personHealthObject.AddComponent<HealthView>();
            var objectHealthView = objectHealthObject.AddComponent<HealthView>();

            camera.transform.position = new Vector3(0, 0, -10);
            camera.orthographic = true;
            personPointObject.transform.SetParent(personObject.transform);
            objectPointObject.transform.SetParent(objectObject.transform);
            personPointObject.transform.position = new Vector3(-2, 1, 0);
            objectPointObject.transform.position = new Vector3(2, 3, 0);
            typeof(PersonView).GetField("m_healthSpawnPoint", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(personView, personPointObject.transform);
            typeof(ObjectView).GetField("m_healthSpawnPoint", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(objectView, objectPointObject.transform);

            systems.Add(new HealthViewFollowSystem(new CameraService(camera, null), registry));

            try
            {
                int personEntity = world.NewEntity();
                int personHealthEntity = world.NewEntity();
                world.GetPool<Health>().Add(personEntity).ViewEntity = personHealthEntity;
                world.GetPool<HealthViewComponent>().Add(personHealthEntity);
                registry.Register(personEntity, personView);
                registry.Register(personHealthEntity, personHealthView);

                int objectEntity = world.NewEntity();
                int objectHealthEntity = world.NewEntity();
                world.GetPool<Health>().Add(objectEntity).ViewEntity = objectHealthEntity;
                world.GetPool<HealthViewComponent>().Add(objectHealthEntity);
                registry.Register(objectEntity, objectView);
                registry.Register(objectHealthEntity, objectHealthView);

                systems.Init();
                systems.Run();

                Vector3 expectedPersonPosition = camera.WorldToScreenPoint(personPointObject.transform.position);
                Vector3 expectedObjectPosition = camera.WorldToScreenPoint(objectPointObject.transform.position);

                Assert.That(personHealthView.transform.position.x, Is.EqualTo(expectedPersonPosition.x).Within(0.001f));
                Assert.That(personHealthView.transform.position.y, Is.EqualTo(expectedPersonPosition.y).Within(0.001f));
                Assert.That(objectHealthView.transform.position.x, Is.EqualTo(expectedObjectPosition.x).Within(0.001f));
                Assert.That(objectHealthView.transform.position.y, Is.EqualTo(expectedObjectPosition.y).Within(0.001f));
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
                registry.Dispose();
                Object.DestroyImmediate(cameraObject);
                Object.DestroyImmediate(personObject);
                Object.DestroyImmediate(objectObject);
                Object.DestroyImmediate(personHealthObject);
                Object.DestroyImmediate(objectHealthObject);
            }
        }

        [Test]
        public void RunSystem_StoresIdleTimerOnEachRunningEntity()
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            var personData = ScriptableObject.CreateInstance<PersonData>();
            var movingObject = new GameObject("Moving body");
            var idleObject = new GameObject("Idle body");
            var runSystem = new RunSystem(personData);

            systems.Add(runSystem);

            try
            {
                int movingEntity = CreateRunningEntity(world, movingObject.AddComponent<Rigidbody2D>());
                int idleEntity = CreateRunningEntity(world, idleObject.AddComponent<Rigidbody2D>());
                ref Run movingRun = ref world.GetPool<Run>().Get(movingEntity);
                ref Run idleRun = ref world.GetPool<Run>().Get(idleEntity);
                movingRun.Direction = 1;
                idleRun.Direction = 0;
                idleRun.IdleTimeRemaining = 7f;

                systems.Init();
                typeof(RunSystem).GetMethod("TryMove", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(runSystem, null);

                Assert.That(movingRun.IdleTimeRemaining, Is.EqualTo(.05f));
                Assert.That(idleRun.IdleTimeRemaining, Is.EqualTo(7f));
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
                Object.DestroyImmediate(personData);
                Object.DestroyImmediate(movingObject);
                Object.DestroyImmediate(idleObject);
            }
        }

        [Test]
        public void GameplayViewFactory_CreatesAllViewsUnderRequestedParent()
        {
            var playerPrefab = new GameObject("Player prefab");
            var boxPrefab = new GameObject("Box prefab");
            var coinPrefab = new GameObject("Coin prefab");
            var healthPrefab = new GameObject("Health prefab");
            var canvasPrefab = new GameObject("Canvas prefab");
            var levelPrefab = new GameObject("Level prefab");
            var counterPrefab = new GameObject("Counter prefab");
            var parent = new GameObject("View parent");

            playerPrefab.AddComponent<PersonView>();
            boxPrefab.AddComponent<ObjectView>();
            coinPrefab.AddComponent<CoinView>();
            healthPrefab.AddComponent<HealthView>();
            canvasPrefab.AddComponent<Canvas>();
            levelPrefab.AddComponent<GameLevelView>();
            counterPrefab.AddComponent<CoinsCounterView>();

            var factory = new GameplayViewFactory(
                canvasPrefab.GetComponent<Canvas>(),
                playerPrefab.GetComponent<PersonView>(),
                boxPrefab.GetComponent<ObjectView>(),
                coinPrefab.GetComponent<CoinView>(),
                healthPrefab.GetComponent<HealthView>(),
                levelPrefab.GetComponent<GameLevelView>(),
                counterPrefab.GetComponent<CoinsCounterView>());

            try
            {
                PersonView player = factory.CreatePlayer(parent.transform);
                ObjectView box = factory.CreateBox(parent.transform);
                CoinView coin = factory.CreateCoin(parent.transform);
                HealthView health = factory.CreateHealth(parent.transform);
                Canvas canvas = factory.CreateCanvas();
                GameLevelView level = factory.CreateGameLevel(parent.transform);
                CoinsCounterView counter = factory.CreateCoinsCounter(parent.transform);

                Assert.That(player.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(box.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(coin.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(health.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(level.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(counter.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(canvas.gameObject, Is.Not.SameAs(canvasPrefab));
                Assert.That(player.gameObject, Is.Not.SameAs(playerPrefab));

                Object.DestroyImmediate(canvas.gameObject);
            }
            finally
            {
                Object.DestroyImmediate(parent);
                Object.DestroyImmediate(playerPrefab);
                Object.DestroyImmediate(boxPrefab);
                Object.DestroyImmediate(coinPrefab);
                Object.DestroyImmediate(healthPrefab);
                Object.DestroyImmediate(canvasPrefab);
                Object.DestroyImmediate(levelPrefab);
                Object.DestroyImmediate(counterPrefab);
            }
        }

        [Test]
        public void BridgeFactories_CreateUiSensorsAndEffectsOutsideEcsSystems()
        {
            var finishPrefab = new GameObject("Finish prefab");
            var tutorialPrefab = new GameObject("Tutorial prefab");
            var sensorPrefab = new GameObject("Sensor prefab");
            var particlesPrefab = new GameObject("Particles prefab");
            var parent = new GameObject("Bridge parent");

            finishPrefab.AddComponent<FinishView>();
            tutorialPrefab.AddComponent<TextMeshProUGUI>();
            sensorPrefab.AddComponent<Sensor>();

            var uiFactory = new GameplayUiBridgeFactory(
                finishPrefab.GetComponent<FinishView>(),
                tutorialPrefab.GetComponent<TextMeshProUGUI>());
            var sensorFactory = new GameplaySensorBridgeFactory(sensorPrefab.GetComponent<Sensor>());
            var effectsFactory = new GameplayEffectsBridgeFactory(particlesPrefab);

            try
            {
                FinishView finish = uiFactory.CreateFinish(parent.transform);
                GameObject tutorial = uiFactory.CreateTutorial(parent.transform);
                Sensor sensor = sensorFactory.CreateSensor(parent.transform, new Vector2(1, 2));
                GameObject particles = effectsFactory.CreateDestroyedParticles(new Vector3(3, 4, 0));

                Assert.That(finish.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(tutorial.transform.parent, Is.EqualTo(parent.transform));
                Assert.That(sensor.transform.localPosition, Is.EqualTo(new Vector3(1, 2, 0)));
                Assert.That(particles.transform.position, Is.EqualTo(new Vector3(3, 4, 0)));
            }
            finally
            {
                Object.DestroyImmediate(parent);
                Object.DestroyImmediate(finishPrefab);
                Object.DestroyImmediate(tutorialPrefab);
                Object.DestroyImmediate(sensorPrefab);
                Object.DestroyImmediate(particlesPrefab);
            }
        }

        [Test]
        public void Sensor_StaysConnectedUntilAllCollidersExit()
        {
            var sensorObject = new GameObject("Sensor");
            var firstColliderObject = new GameObject("First collider");
            var secondColliderObject = new GameObject("Second collider");
            var sensor = sensorObject.AddComponent<Sensor>();
            var firstCollider = firstColliderObject.AddComponent<BoxCollider2D>();
            var secondCollider = secondColliderObject.AddComponent<BoxCollider2D>();

            try
            {
                InvokeSensorTrigger(sensor, "OnTriggerEnter2D", firstCollider);
                InvokeSensorTrigger(sensor, "OnTriggerEnter2D", secondCollider);
                InvokeSensorTrigger(sensor, "OnTriggerExit2D", firstCollider);

                Assert.That(sensor.IsConnected, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(sensorObject);
                Object.DestroyImmediate(firstColliderObject);
                Object.DestroyImmediate(secondColliderObject);
            }
        }

        [UnityTest]
        public IEnumerator Sensor_ReenterCancelsPendingExit()
        {
            var sensorObject = new GameObject("Sensor");
            var colliderObject = new GameObject("Collider");
            var sensor = sensorObject.AddComponent<Sensor>();
            var collider = colliderObject.AddComponent<BoxCollider2D>();

            try
            {
                InvokeSensorTrigger(sensor, "OnTriggerEnter2D", collider);
                InvokeSensorTrigger(sensor, "OnTriggerExit2D", collider);
                InvokeSensorTrigger(sensor, "OnTriggerEnter2D", collider);

                yield return new WaitForSecondsRealtime(0.2f);

                Assert.That(sensor.IsConnected, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(sensorObject);
                Object.DestroyImmediate(colliderObject);
            }
        }

        [UnityTest]
        public IEnumerator Sensor_DestroyingViewCancelsPendingExit()
        {
            var sensorObject = new GameObject("Sensor");
            var colliderObject = new GameObject("Collider");
            var sensor = sensorObject.AddComponent<Sensor>();
            var collider = colliderObject.AddComponent<BoxCollider2D>();

            try
            {
                InvokeSensorTrigger(sensor, "OnTriggerEnter2D", collider);
                InvokeSensorTrigger(sensor, "OnTriggerExit2D", collider);

                InvokeSensorLifecycle(sensor, "OnDestroy");
                Object.DestroyImmediate(sensorObject);

                Assert.That(GetPendingExitCancellationTokenSource(sensor), Is.Null);

                yield return new WaitForSecondsRealtime(0.2f);
            }
            finally
            {
                if (sensorObject != null)
                    Object.DestroyImmediate(sensorObject);

                Object.DestroyImmediate(colliderObject);
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

        private static void InvokeSensorTrigger(Sensor sensor, string methodName, Collider2D collider)
        {
            var method = typeof(Sensor).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(sensor, new object[] { collider });
        }

        private static int CreateRunningEntity(EcsWorld world, Rigidbody2D rigidbody)
        {
            int entity = world.NewEntity();
            world.GetPool<Run>().Add(entity);
            world.GetPool<Rigidbody2d>().Add(entity).Rigidbody = rigidbody;
            world.GetPool<WallCheck>().Add(entity);
            world.GetPool<GroundCheckComponent>().Add(entity);
            return entity;
        }

        private static object GetPendingExitCancellationTokenSource(Sensor sensor)
        {
            var field = typeof(Sensor).GetField("m_pendingExitCancellationTokenSource", BindingFlags.Instance | BindingFlags.NonPublic);
            return field.GetValue(sensor);
        }

        private static void InvokeSensorLifecycle(Sensor sensor, string methodName)
        {
            var method = typeof(Sensor).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(sensor, null);
        }

        private sealed class SessionLifecycleProbeSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
        {
            private System.Threading.CancellationToken m_cancellationToken;
            private SessionOperationProbe m_sessionOperation;

            public int InitCallCount { get; private set; }

            public int RunCallCount { get; private set; }

            public int DestroyCallCount { get; private set; }

            public bool CancellationWasRequestedDuringDestroy { get; private set; }

            public bool SessionOperationWasCanceledDuringDestroy { get; private set; }

            public bool WorldWasAliveDuringDestroy { get; private set; }

            public void SetCancellationToken(System.Threading.CancellationToken cancellationToken)
            {
                m_cancellationToken = cancellationToken;
            }

            public void SetSessionOperation(SessionOperationProbe sessionOperation)
            {
                m_sessionOperation = sessionOperation;
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
                SessionOperationWasCanceledDuringDestroy = m_sessionOperation.IsCanceled;
                WorldWasAliveDuringDestroy = systems.GetWorld().IsAlive();
            }
        }

        private sealed class InputReaderProbe : IGameplayInputReader
        {
            public GameplayInputSnapshot Snapshot { get; set; }

            public void UpdateSnapshot()
            {
            }
        }

        private sealed class SessionOperationProbe : IGameSessionOperation
        {
            public int CancelCallCount { get; private set; }

            public bool IsCanceled => CancelCallCount > 0;

            public void Cancel()
            {
                CancelCallCount++;
            }
        }

        private sealed class ThrowingSessionOperation : IGameSessionOperation
        {
            public void Cancel()
            {
                throw new System.InvalidOperationException();
            }
        }
    }
}
