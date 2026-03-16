using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.HealthViewService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewInitSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private EcsWorld m_world;

        private EcsFilter m_healthFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<HealthViewComponent> m_healthViewPool;

        private readonly HealthView m_healthViewPrefab;
        private readonly ICanvasService m_canvasService;
        private readonly IHealthViewService m_healthViewService;

        public HealthViewInitSystem(HealthView healthViewPrefab, IHealthViewService healthViewService, ICanvasService canvasService)
        {
            m_canvasService = canvasService;
            m_healthViewPrefab = healthViewPrefab;
            m_healthViewService = healthViewService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_healthFilter = m_world.Filter<Health>().End();

            m_healthPool = m_world.GetPool<Health>();
            m_healthViewPool = m_world.GetPool<HealthViewComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CreateHealthView();
        }

        public void Destroy(IEcsSystems systems)
        {
            /*foreach (var healthViewEntity in m_healthViewFilter)
                Object.Destroy(m_healthViewPool.Get(healthViewEntity).HealthView);*/
        }

        private void CreateHealthView()
        {
            foreach (var readyToHealthViewEntity in m_healthFilter)
            {
                ref Health health = ref m_healthPool.Get(readyToHealthViewEntity);
                if(health.ViewEntity != 0)
                    continue;
                
                var newEntity = m_world.NewEntity();
                
                var spawnPoint = m_canvasService.Canvas.transform;
                    
                var healthView = Object.Instantiate(m_healthViewPrefab, spawnPoint);
                SetViewOptions(healthView, health);

                m_healthViewService.AddHealthView(newEntity, healthView);
                
                m_healthViewPool.Add(newEntity);
                AttachTransformComponent(newEntity, healthView.transform);
                
                health.ViewEntity = newEntity;
            }
        }

        private void AttachTransformComponent(int entity, Transform attachedTransform)
        {
            ref TransformKeeper transformKeeper = ref m_world.GetPool<TransformKeeper>().Add(entity);
            transformKeeper.ObjectTransform = attachedTransform;
        }

        private void SetViewOptions(HealthView healthView, Health health)
        {
            healthView.CanvasGroup.alpha = 0;
            healthView.transform.localPosition = Vector3.zero;
            healthView.HealthBar.maxValue = health.Count;
        }
    }
}