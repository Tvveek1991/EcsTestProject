using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.GameLevelService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class PlayerInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string PlayerParentName = "Hero";
        
        private readonly PersonView m_personViewPrefab;
        private readonly IGameLevelService m_gameLevelService;

        private EcsWorld m_world;

        private EcsFilter m_playerTransformFilter;

        private EcsPool<TransformKeeper> m_transformPool;
        
        private GameObject m_parentObject;
        
        public PlayerInitSystem(PersonView personViewPrefab, IGameLevelService gameLevelService)
        {
            m_personViewPrefab = personViewPrefab;
            m_gameLevelService = gameLevelService;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_playerTransformFilter = m_world.Filter<Player>().Inc<TransformKeeper>().End(1);
            
            m_transformPool = m_world.GetPool<TransformKeeper>();

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
                m_world.GetPool<Player>().Add(playerEntity);
            }

            void AttachPersonComponent()
            {
                m_world.GetPool<Person>().Add(playerEntity);
            }

            void AttachAnimatorComponent()
            {
                ref AnimatorKeeper animatorKeeper = ref m_world.GetPool<AnimatorKeeper>().Add(playerEntity);
                animatorKeeper.AnimatorController = heroView.GetComponent<Animator>();
            }

            void AttachTransformComponent()
            {
                ref TransformKeeper transformKeeper = ref m_world.GetPool<TransformKeeper>().Add(playerEntity);
                transformKeeper.ObjectTransform = heroView.transform;
            }

            void AttachRigidbody2dComponent()
            {
                ref Rigidbody2d rigidbody2d = ref m_world.GetPool<Rigidbody2d>().Add(playerEntity);
                rigidbody2d.Rigidbody = heroView.GetComponent<Rigidbody2D>();
            }

            void AttachSpriteRendererComponent()
            {
                ref SpriteRendererKeeper spriteRendererKeeper = ref m_world.GetPool<SpriteRendererKeeper>().Add(playerEntity);
                spriteRendererKeeper.SpriteRenderer = heroView.GetComponent<SpriteRenderer>();
            }

            void AttachViewToHeroViewReferenceComponent()
            {
                ref PersonViewRef cellViewRef = ref m_world.GetPool<PersonViewRef>().Add(playerEntity);
                cellViewRef.PersonView = heroView;
            }

            void AttachGroundCheckComponent()
            {
                m_world.GetPool<GroundCheckComponent>().Add(playerEntity);
            }

            void AttachWallCheckComponent()
            {
                m_world.GetPool<WallCheck>().Add(playerEntity);
            }
        }

        private void SetSpawnPosition()
        {
            foreach (var player in m_playerTransformFilter)
            {
                m_transformPool.Get(player).ObjectTransform.position = m_gameLevelService.View.GetHeroSpawnPoint();
            }
        }
    }
}
