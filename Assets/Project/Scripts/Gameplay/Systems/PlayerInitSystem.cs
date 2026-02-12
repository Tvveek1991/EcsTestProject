using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class PlayerInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string PlayerParentName = "Hero";
        
        private readonly PersonView m_personViewPrefab;
        
        private EcsWorld m_world;

        private EcsFilter m_playerTransformFilter;
        private EcsFilter m_gameLevelViewRefsFilter;

        private EcsPool<TransformComponent> m_transformPool;
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;
        
        private GameObject m_parentObject;
        
        public PlayerInitSystem(PersonView personViewPrefab)
        {
            m_personViewPrefab = personViewPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_playerTransformFilter = m_world.Filter<PlayerComponent>().Inc<TransformComponent>().End();
            m_gameLevelViewRefsFilter = m_world.Filter<GameLevelViewRefComponent>().End();
            m_transformPool = m_world.GetPool<TransformComponent>();
            m_gameLevelViewRefsPool = m_world.GetPool<GameLevelViewRefComponent>();

            CreateHeroView();
            SetSpawnPosition();
        }

        public void Destroy(IEcsSystems systems) => 
            Object.Destroy(m_parentObject);

        private void CreateHeroView()
        {
            m_parentObject = new GameObject(PlayerParentName);
            
            var playerEntity = m_world.NewEntity();

            var heroView = Object.Instantiate(m_personViewPrefab, m_parentObject.transform).GetComponent<PersonView>();

            AttachComponents(playerEntity, heroView);
        }

        private void AttachComponents(int playerEntity, PersonView heroView)
        {
            AttachPlayerComponent();
            AttachPersonComponent();
            AttachAnimatorComponent();
            AttachTransformComponent();
            AttachWallCheckComponent();
            AttachGroundCheckComponent();
            AttachRigidbody2dComponent();
            AttachSpriteRendererComponent();
            AttachViewToHeroViewReferenceComponent();

            void AttachPlayerComponent()
            {
                m_world.GetPool<PlayerComponent>().Add(playerEntity);
            }

            void AttachPersonComponent()
            {
                m_world.GetPool<PersonComponent>().Add(playerEntity);
            }

            void AttachAnimatorComponent()
            {
                ref AnimatorComponent animatorComponent = ref m_world.GetPool<AnimatorComponent>().Add(playerEntity);
                animatorComponent.AnimatorController = heroView.GetComponent<Animator>();
            }

            void AttachTransformComponent()
            {
                ref TransformComponent transformComponent = ref m_world.GetPool<TransformComponent>().Add(playerEntity);
                transformComponent.ObjectTransform = heroView.transform;
            }

            void AttachRigidbody2dComponent()
            {
                ref Rigidbody2dComponent rigidbody2dComponent = ref m_world.GetPool<Rigidbody2dComponent>().Add(playerEntity);
                rigidbody2dComponent.Rigidbody = heroView.GetComponent<Rigidbody2D>();
            }

            void AttachSpriteRendererComponent()
            {
                ref SpriteRendererComponent spriteRendererComponent = ref m_world.GetPool<SpriteRendererComponent>().Add(playerEntity);
                spriteRendererComponent.SpriteRenderer = heroView.GetComponent<SpriteRenderer>();
            }

            void AttachViewToHeroViewReferenceComponent()
            {
                ref PersonViewRefComponent cellViewRef = ref m_world.GetPool<PersonViewRefComponent>().Add(playerEntity);
                cellViewRef.PersonView = heroView;
            }

            void AttachGroundCheckComponent()
            {
                m_world.GetPool<GroundCheckComponent>().Add(playerEntity);
            }

            void AttachWallCheckComponent()
            {
                m_world.GetPool<WallCheckComponent>().Add(playerEntity);
            }
        }

        private void SetSpawnPosition()
        {
            foreach (var gameLevel in m_gameLevelViewRefsFilter)
            foreach (var player in m_playerTransformFilter)
            {
                m_transformPool.Get(player).ObjectTransform.position = m_gameLevelViewRefsPool.Get(gameLevel).GameLevelView.GetHeroSpawnPoint();
            }
        }
    }
}
