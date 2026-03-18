using System.Collections.Generic;
using Gameplay.Services.ObjectsService;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.GameLevelService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class BoxViewInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string BoxParentName = "Boxes";

        private readonly ObjectView m_objectViewPrefab;
        private readonly IObjectsService m_objectsService;
        private readonly IGameLevelService m_gameLevelService;

        private EcsWorld m_world;

        private EcsFilter m_boxTransformFilter;

        private EcsPool<TransformKeeper> m_transformPool;

        private GameObject m_parentObject;
        private List<Transform> m_boxSpawnPoints;

        public BoxViewInitSystem(ObjectView objectViewPrefab, IObjectsService objectsService, IGameLevelService gameLevelService)
        {
            m_objectsService = objectsService;
            m_objectViewPrefab = objectViewPrefab;
            m_gameLevelService = gameLevelService;
        }

        public void Init(IEcsSystems systems)
        {
            m_objectsService.Clear();
            
            m_world = systems.GetWorld();

            m_boxTransformFilter = m_world.Filter<PlayableObject>().Inc<TransformKeeper>().End();
            
            m_transformPool = m_world.GetPool<TransformKeeper>();
            
            CreateBoxViews();
            
            SetBoxStartPosition();
        }

        public void Destroy(IEcsSystems systems)
        {
            m_objectsService.Clear();
            
            Object.Destroy(m_parentObject);
        }

        private void CreateBoxViews()
        {
            m_parentObject = new GameObject(BoxParentName);
            m_boxSpawnPoints = m_gameLevelService.View.GetBoxSpawnPoints();

            for (int i = 0; i < m_boxSpawnPoints.Count; i++)
            {
                var entity = m_world.NewEntity();
                var view = Object.Instantiate(m_objectViewPrefab, m_parentObject.transform);
                
                m_objectsService.AddObjectView(entity, view);
                
                AttachComponents(entity, view);
            }
        }

        private void AttachComponents(int entity, ObjectView view)
        {
            m_world.GetPool<ObjectViewComponent>().Add(entity);
            m_world.GetPool<PlayableObject>().Add(entity);
            
            AttachTransformComponent();
            AttachRigidbody2dComponent();
            AttachSpriteRendererComponent();

            void AttachTransformComponent()
            {
                ref TransformKeeper transformKeeper = ref m_world.GetPool<TransformKeeper>().Add(entity);
                transformKeeper.ObjectTransform = view.transform;
            }

            void AttachRigidbody2dComponent()
            {
                ref Rigidbody2d rigidbody2d = ref m_world.GetPool<Rigidbody2d>().Add(entity);
                rigidbody2d.Rigidbody = view.GetComponent<Rigidbody2D>();
            }

            void AttachSpriteRendererComponent()
            {
                ref SpriteRendererKeeper spriteRendererKeeper = ref m_world.GetPool<SpriteRendererKeeper>().Add(entity);
                spriteRendererKeeper.SpriteRenderer = view.GetComponent<SpriteRenderer>();
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