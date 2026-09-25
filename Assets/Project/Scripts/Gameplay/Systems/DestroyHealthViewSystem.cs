using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyHealthViewSystem : IEcsInitSystem, IEcsPostRunSystem
    {
        private readonly IEntityViewRegistry m_entityViewRegistry;

        private EcsWorld m_world;

        private EcsFilter m_deadFilter;

        private EcsPool<Health> m_healthPool;

        public DestroyHealthViewSystem(IEntityViewRegistry entityViewRegistry)
        {
            m_entityViewRegistry = entityViewRegistry;
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
                
                if (!m_entityViewRegistry.TryGet(health.ViewEntity, out HealthView view))
                    continue;
                
                m_entityViewRegistry.Unregister(health.ViewEntity);
                Object.Destroy(view.gameObject);
                
                m_world.DelEntity(health.ViewEntity);
            }
        }
    }
}
