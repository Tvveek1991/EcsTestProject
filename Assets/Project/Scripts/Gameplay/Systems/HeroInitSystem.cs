using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class HeroInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string HeroParentName = "Hero";
        
        private readonly HeroView m_heroViewPrefab;
        
        private EcsWorld m_world;
        
        private EcsFilter m_gameLevelViewRefsFilter;
        
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;
        
        private GameObject m_parentObject;
        
        public HeroInitSystem(HeroView heroViewPrefab)
        {
            m_heroViewPrefab = heroViewPrefab;
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
            m_parentObject = new GameObject(HeroParentName);
            
            var gameLevelEntityIndex = m_world.NewEntity();

            var heroView = Object.Instantiate(m_heroViewPrefab, m_parentObject.transform).GetComponent<HeroView>();

            SetSpawnPosition(heroView);
            
            AttachHeroComponent();
            AttachSpriteRendererComponent();
            AttachTransformComponent();
            AttachFlipViewComponent();
            AttachRunComponent();
            AttachAttackComponent();
            AttachGroundCheckComponent();
            AttachWallCheckComponent();
            AttachAnimatorComponent();
            AttachRigidbody2dComponent();
            AttachViewToHeroViewReferenceComponent();

            void AttachHeroComponent()
            {
                m_world.GetPool<HeroComponent>().Add(gameLevelEntityIndex);
            }

            void AttachSpriteRendererComponent()
            {
                ref SpriteRendererComponent spriteRendererComponent = ref m_world.GetPool<SpriteRendererComponent>().Add(gameLevelEntityIndex);
                spriteRendererComponent.SpriteRenderer = heroView.GetComponent<SpriteRenderer>();
            }

            void AttachTransformComponent()
            {
                ref TransformComponent transformComponent = ref m_world.GetPool<TransformComponent>().Add(gameLevelEntityIndex);
                transformComponent.ObjectTransform = heroView.transform;
            }

            void AttachFlipViewComponent()
            {
                m_world.GetPool<FlipViewComponent>().Add(gameLevelEntityIndex);
            }
            
            void AttachGroundCheckComponent()
            {
                m_world.GetPool<GroundCheckComponent>().Add(gameLevelEntityIndex);
            }
            
            void AttachWallCheckComponent()
            {
                m_world.GetPool<WallCheckComponent>().Add(gameLevelEntityIndex);
            }
            
            void AttachRunComponent()
            {
                m_world.GetPool<RunComponent>().Add(gameLevelEntityIndex);
            }
            
            void AttachAttackComponent()
            {
                m_world.GetPool<AttackComponent>().Add(gameLevelEntityIndex);
            }
            
            void AttachAnimatorComponent()
            {
                ref AnimatorComponent animatorComponent = ref m_world.GetPool<AnimatorComponent>().Add(gameLevelEntityIndex);
                animatorComponent.AnimatorController = heroView.GetComponent<Animator>();
            }
            
            void AttachRigidbody2dComponent()
            {
                ref Rigidbody2dComponent rigidbody2dComponent = ref m_world.GetPool<Rigidbody2dComponent>().Add(gameLevelEntityIndex);
                rigidbody2dComponent.Rigidbody = heroView.GetComponent<Rigidbody2D>();
            }

            void AttachViewToHeroViewReferenceComponent()
            {
                ref HeroViewRefComponent cellViewRef = ref m_world.GetPool<HeroViewRefComponent>().Add(gameLevelEntityIndex);
                cellViewRef.HeroView = heroView;
            }
        }

        private void SetSpawnPosition(HeroView heroView)
        {
            foreach (var item in m_gameLevelViewRefsFilter)
            {
                heroView.SetPosition(m_gameLevelViewRefsPool.Get(item).GameLevelView.GetHeroSpawnPoint());
            }
        }
    }
}
