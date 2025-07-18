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
        
        private EcsFilter m_gameLevelViewRefsFilter;
        
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;
        
        private GameObject m_parentObject;
        
        public PlayerInitSystem(PersonView personViewPrefab)
        {
            m_personViewPrefab = personViewPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_gameLevelViewRefsFilter = m_world.Filter<GameLevelViewRefComponent>().End();
            m_gameLevelViewRefsPool = m_world.GetPool<GameLevelViewRefComponent>();

            CreateHeroView();
        }

        public void Destroy(IEcsSystems systems) => 
            Object.Destroy(m_parentObject);

        private void CreateHeroView()
        {
            m_parentObject = new GameObject(PlayerParentName);
            
            var gameLevelEntityIndex = m_world.NewEntity();

            var heroView = Object.Instantiate(m_personViewPrefab, m_parentObject.transform).GetComponent<PersonView>();

            SetSpawnPosition(heroView);

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
                m_world.GetPool<PlayerComponent>().Add(gameLevelEntityIndex);
            }
            
            void AttachPersonComponent()
            {
                m_world.GetPool<PersonComponent>().Add(gameLevelEntityIndex);
            }
            
            void AttachAnimatorComponent()
            {
                ref AnimatorComponent animatorComponent = ref m_world.GetPool<AnimatorComponent>().Add(gameLevelEntityIndex);
                animatorComponent.AnimatorController = heroView.GetComponent<Animator>();
            }

            void AttachTransformComponent()
            {
                ref TransformComponent transformComponent = ref m_world.GetPool<TransformComponent>().Add(gameLevelEntityIndex);
                transformComponent.ObjectTransform = heroView.transform;
            }

            void AttachRigidbody2dComponent()
            {
                ref Rigidbody2dComponent rigidbody2dComponent = ref m_world.GetPool<Rigidbody2dComponent>().Add(gameLevelEntityIndex);
                rigidbody2dComponent.Rigidbody = heroView.GetComponent<Rigidbody2D>();
            }
            
            void AttachSpriteRendererComponent()
            {
                ref SpriteRendererComponent spriteRendererComponent = ref m_world.GetPool<SpriteRendererComponent>().Add(gameLevelEntityIndex);
                spriteRendererComponent.SpriteRenderer = heroView.GetComponent<SpriteRenderer>();
            }

            void AttachViewToHeroViewReferenceComponent()
            {
                ref PersonViewRefComponent cellViewRef = ref m_world.GetPool<PersonViewRefComponent>().Add(gameLevelEntityIndex);
                cellViewRef.PersonView = heroView;
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

        private void SetSpawnPosition(PersonView personView)
        {
            foreach (var item in m_gameLevelViewRefsFilter)
            {
                personView.SetPosition(m_gameLevelViewRefsPool.Get(item).GameLevelView.GetHeroSpawnPoint());
            }
        }
    }
}
