using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class BoxInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string BoxParentName = "Boxes";

        private readonly ObjectView m_objectViewPrefab;

        private EcsWorld m_world;

        private EcsFilter m_boxTransformFilter;
        private EcsFilter m_gameLevelViewRefsFilter;

        private EcsPool<TransformComponent> m_transformPool;
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;

        private GameObject m_parentObject;
        private List<Transform> m_boxSpawnPoints;

        public BoxInitSystem(ObjectView objectViewPrefab)
        {
            m_objectViewPrefab = objectViewPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_gameLevelViewRefsFilter = m_world.Filter<GameLevelViewRefComponent>().End();
            m_boxTransformFilter = m_world.Filter<ObjectComponent>().Inc<TransformComponent>().End();
            
            m_transformPool = m_world.GetPool<TransformComponent>();
            m_gameLevelViewRefsPool = m_world.GetPool<GameLevelViewRefComponent>();

            FindSpawnPoints();
            CreateBoxViews();
            
            SetBoxStartPosition();
        }

        public void Destroy(IEcsSystems systems) =>
            Object.Destroy(m_parentObject);

        private void CreateBoxViews()
        {
            m_parentObject = new GameObject(BoxParentName);

            for (int i = 0; i < m_boxSpawnPoints.Count; i++)
            {
                var gameLevelEntityIndex = m_world.NewEntity();
                var boxView = Object.Instantiate(m_objectViewPrefab, m_parentObject.transform).GetComponent<ObjectView>();
                
                AttachComponents(gameLevelEntityIndex, boxView);
            }
        }

        private void AttachComponents(int gameLevelEntityIndex, ObjectView objectView)
        {
            

            AttachObjectComponent();
            // AttachPersonComponent();
            // AttachAnimatorComponent();
            AttachTransformComponent();
            // AttachWallCheckComponent();
            // AttachGroundCheckComponent();
            AttachRigidbody2dComponent();
            AttachSpriteRendererComponent();
            AttachViewToHeroViewReferenceComponent();

            void AttachObjectComponent()
            {
                m_world.GetPool<ObjectComponent>().Add(gameLevelEntityIndex);
            }

            void AttachAnimatorComponent()
            {
                ref AnimatorComponent animatorComponent = ref m_world.GetPool<AnimatorComponent>().Add(gameLevelEntityIndex);
                animatorComponent.AnimatorController = objectView.GetComponent<Animator>();
            }

            void AttachTransformComponent()
            {
                ref TransformComponent transformComponent = ref m_world.GetPool<TransformComponent>().Add(gameLevelEntityIndex);
                transformComponent.ObjectTransform = objectView.transform;
            }

            void AttachRigidbody2dComponent()
            {
                ref Rigidbody2dComponent rigidbody2dComponent = ref m_world.GetPool<Rigidbody2dComponent>().Add(gameLevelEntityIndex);
                rigidbody2dComponent.Rigidbody = objectView.GetComponent<Rigidbody2D>();
            }

            void AttachSpriteRendererComponent()
            {
                ref SpriteRendererComponent spriteRendererComponent = ref m_world.GetPool<SpriteRendererComponent>().Add(gameLevelEntityIndex);
                spriteRendererComponent.SpriteRenderer = objectView.GetComponent<SpriteRenderer>();
            }

            void AttachViewToHeroViewReferenceComponent()
            {
                ref ObjectViewRefComponent cellViewRef = ref m_world.GetPool<ObjectViewRefComponent>().Add(gameLevelEntityIndex);
                cellViewRef.ObjectView = objectView;
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
                m_boxSpawnPoints = gameLevelViewRefComponent.GameLevelView.GetBoxSpawnPoints();
            }
        }
        
        private void SetBoxStartPosition()
        {
            var listSpawnPoints = m_boxSpawnPoints;
            foreach (var box in m_boxTransformFilter)
            {
                if (listSpawnPoints.Count <= 0) continue;
                
                m_transformPool.Get(box).ObjectTransform.position = listSpawnPoints[0].position;
                listSpawnPoints.RemoveAt(0);
            }
        }
    }
}