using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.HealthViewService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyHealthViewSystem : IEcsInitSystem, IEcsPostRunSystem
    {
        private readonly IHealthViewService m_healthViewService;

        private EcsWorld m_world;

        private EcsFilter m_deadFilter;

        private EcsPool<Health> m_healthPool;

        public DestroyHealthViewSystem(IHealthViewService healthViewService)
        {
            m_healthViewService = healthViewService;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_deadFilter = m_world.Filter<Health>().Inc<Dead>().End();
            
            m_healthPool = m_world.GetPool<Health>();
        }
 
        public void PostRun(IEcsSystems systems)
        {
            foreach (var entity in m_deadFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);
                
                if (!m_healthViewService.Views.TryGetValue(health.ViewEntity, out var view))
                    continue;
                
                m_healthViewService.RemoveView(health.ViewEntity);
                Object.Destroy(view.gameObject);
                
                m_world.DelEntity(health.ViewEntity);
            }
        }
    }
}