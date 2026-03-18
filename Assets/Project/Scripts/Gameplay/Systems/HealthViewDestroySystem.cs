using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Serializabled;
using Project.Scripts.Gameplay.Services.HealthViewService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewDestroySystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private readonly IHealthViewService m_healthViewService;

        private EcsWorld m_world;

        private EcsFilter m_deadFilter;
        private EcsFilter m_deadCommandFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<DeadCommand> m_deadCommandPool;

        public HealthViewDestroySystem(IHealthViewService healthViewService)
        {
            m_healthViewService = healthViewService;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_deadFilter = m_world.Filter<Dead>().End();
            m_deadCommandFilter = m_world.Filter<DeadCommand>().End();
            
            m_healthPool = m_world.GetPool<Health>();
            m_deadCommandPool = m_world.GetPool<DeadCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in m_deadCommandFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);
                
                if (!m_healthViewService.Views.TryGetValue(health.ViewEntity, out var view))
                    continue;

                if(m_deadCommandPool.Get(entity).HealthViewDestroyedStatus != ProcessStatus.Ready)
                    continue;

                m_healthViewService.RemoveView(health.ViewEntity);
                Object.Destroy(view.gameObject);
                
                m_deadCommandPool.Get(entity).HealthViewDestroyedStatus = ProcessStatus.Completed;
            }
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (var entity in m_deadFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);
                
                m_world.DelEntity(health.ViewEntity);
                m_world.DelEntity(entity);
            }
        }
    }
}