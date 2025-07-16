using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class EnemyInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string EnemiesParentName = "Enemies";

        private readonly PersonView m_personViewPrefab;

        private EcsWorld m_world;

        private EcsFilter m_gameLevelViewRefsFilter;

        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;

        private GameObject m_parentObject;
        private List<Transform> m_enemySpawnPoints;

        public EnemyInitSystem(PersonView personViewPrefab)
        {
            m_personViewPrefab = personViewPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_gameLevelViewRefsFilter = m_world.Filter<GameLevelViewRefComponent>().End();
            m_gameLevelViewRefsPool = m_world.GetPool<GameLevelViewRefComponent>();

            FindSpawnPoints();
            CreateEnemyViews();
        }

        public void Destroy(IEcsSystems systems) =>
            Object.Destroy(m_parentObject);

        private void CreateEnemyViews()
        {
            m_parentObject = new GameObject(EnemiesParentName);

            foreach (var spawnPoint in m_enemySpawnPoints)
            {
                var gameLevelEntityIndex = m_world.NewEntity();
                var enemyView = Object.Instantiate(m_personViewPrefab, m_parentObject.transform).GetComponent<PersonView>();
                enemyView.SetPosition(spawnPoint.position);
                
                AttachComponents(gameLevelEntityIndex, enemyView);
            }
        }

        private void AttachComponents(int gameLevelEntityIndex, PersonView enemyView)
        {
            AttachEnemyComponent();
            AttachPersonComponent();
            AttachAnimatorComponent();
            AttachTransformComponent();
            AttachWallCheckComponent();
            AttachGroundCheckComponent();
            AttachRigidbody2dComponent();
            AttachSpriteRendererComponent();
            AttachViewToHeroViewReferenceComponent();

            void AttachEnemyComponent()
            {
                m_world.GetPool<EnemyComponent>().Add(gameLevelEntityIndex);
            }

            void AttachPersonComponent()
            {
                m_world.GetPool<PersonComponent>().Add(gameLevelEntityIndex);
            }

            void AttachAnimatorComponent()
            {
                ref AnimatorComponent animatorComponent = ref m_world.GetPool<AnimatorComponent>().Add(gameLevelEntityIndex);
                animatorComponent.AnimatorController = enemyView.GetComponent<Animator>();
            }

            void AttachTransformComponent()
            {
                ref TransformComponent transformComponent = ref m_world.GetPool<TransformComponent>().Add(gameLevelEntityIndex);
                transformComponent.ObjectTransform = enemyView.transform;
            }

            void AttachRigidbody2dComponent()
            {
                ref Rigidbody2dComponent rigidbody2dComponent = ref m_world.GetPool<Rigidbody2dComponent>().Add(gameLevelEntityIndex);
                rigidbody2dComponent.Rigidbody = enemyView.GetComponent<Rigidbody2D>();
            }

            void AttachSpriteRendererComponent()
            {
                ref SpriteRendererComponent spriteRendererComponent = ref m_world.GetPool<SpriteRendererComponent>().Add(gameLevelEntityIndex);
                spriteRendererComponent.SpriteRenderer = enemyView.GetComponent<SpriteRenderer>();
            }

            void AttachViewToHeroViewReferenceComponent()
            {
                ref PersonViewRefComponent cellViewRef = ref m_world.GetPool<PersonViewRefComponent>().Add(gameLevelEntityIndex);
                cellViewRef.PersonView = enemyView;
            }

            void AttachGroundCheckComponent()
            {
                m_world.GetPool<GroundCheckComponent>().Add(gameLevelEntityIndex);
            }

            void AttachWallCheckComponent()
            {
                m_world.GetPool<WallCheckComponent>().Add(gameLevelEntityIndex);
            }
        }

        private void FindSpawnPoints()
        {
            foreach (var item in m_gameLevelViewRefsFilter)
            {
                ref GameLevelViewRefComponent gameLevelViewRefComponent = ref m_gameLevelViewRefsPool.Get(item);
                m_enemySpawnPoints = gameLevelViewRefComponent.GameLevelView.GetEnemySpawnPoints();
            }
        }
    }
}