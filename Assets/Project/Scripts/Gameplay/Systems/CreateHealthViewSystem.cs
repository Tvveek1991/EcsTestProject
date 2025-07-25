﻿using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CreateHealthViewSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_canvasFilter;
        private EcsFilter m_healthViewFilter;
        private EcsFilter m_healthReadyToViewFilter;

        private EcsPool<CanvasComponent> m_canvasPool;
        private EcsPool<HealthComponent> m_healthPool;
        private EcsPool<HealthViewRefComponent> m_healthViewPool;

        private readonly HealthView m_healthViewPrefab;

        public CreateHealthViewSystem(HealthView healthViewPrefab)
        {
            m_healthViewPrefab = healthViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_canvasFilter = m_world.Filter<CanvasComponent>().End();
            m_healthReadyToViewFilter = m_world.Filter<HealthComponent>().End();
            m_healthViewFilter = m_world.Filter<HealthViewRefComponent>().End();

            m_canvasPool = m_world.GetPool<CanvasComponent>();
            m_healthPool = m_world.GetPool<HealthComponent>();
            m_healthViewPool = m_world.GetPool<HealthViewRefComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CreateHealthView();
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (var healthViewEntity in m_healthViewFilter)
                Object.Destroy(m_healthViewPool.Get(healthViewEntity).HealthView);
        }

        private void CreateHealthView()
        {
            foreach (var readyToHealthViewEntity in m_healthReadyToViewFilter)
            foreach (var canvasEntity in m_canvasFilter)
            {
                ref HealthComponent healthComponent = ref m_healthPool.Get(readyToHealthViewEntity);
                if(healthComponent.HealthViewEntityIndex != 0)
                    continue;
                
                var newEntity = m_world.NewEntity();
                
                var spawnPoint = m_canvasPool.Get(canvasEntity).Canvas.transform;
                    
                var healthView = Object.Instantiate(m_healthViewPrefab, spawnPoint).GetComponent<HealthView>();
                SetViewOptions(healthView, healthComponent);

                AttacheHealthViewComponent(newEntity, healthView);
                AttachTransformComponent(newEntity, healthView.transform);
                
                healthComponent.HealthViewEntityIndex = newEntity;
            }
        }

        private void AttacheHealthViewComponent(int newEntity, HealthView healthView)
        {
            ref HealthViewRefComponent view = ref m_healthViewPool.Add(newEntity);
            view.HealthView = healthView;
        }

        private void AttachTransformComponent(int entity, Transform attachedTransform)
        {
            ref TransformComponent transformComponent = ref m_world.GetPool<TransformComponent>().Add(entity);
            transformComponent.ObjectTransform = attachedTransform;
        }

        private void SetViewOptions(HealthView healthView, HealthComponent healthComponent)
        {
            healthView.CanvasGroup.alpha = 0;
            healthView.transform.localPosition = Vector3.zero;
            healthView.HealthBar.maxValue = healthComponent.Health;
        }
    }
}