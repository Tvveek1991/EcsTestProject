using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_healthViewFilter;
        
        private EcsPool<HealthViewRefComponent> m_healthViewPool;
        private EcsPool<HitHealthCommandComponent> m_hitHealthCommandPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_healthViewFilter = m_world.Filter<HealthViewRefComponent>().Exc<HitHealthCommandComponent>().End();
            
            m_healthViewPool = m_world.GetPool<HealthViewRefComponent>();
            m_hitHealthCommandPool = m_world.GetPool<HitHealthCommandComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckHit();
        }

        private void CheckHit()
        {
            foreach (var entity in m_healthViewFilter)
            {
                ref HealthViewRefComponent healthViewRefComponent = ref m_healthViewPool.Get(entity);
                ref HitHealthCommandComponent hitHealthCommandComponent = ref m_hitHealthCommandPool.Get(entity);

                // healthViewRefComponent.HealthView.CanvasGroup
                
                
                
                
                m_hitHealthCommandPool.Del(entity);
            }
        }
    }
}