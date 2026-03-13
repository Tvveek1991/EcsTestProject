using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCounterChangeSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_coinsCounterFilter;
        private EcsFilter m_coinsCounterChangeFilter;

        private EcsPool<CoinsCounter> m_coinsCounterPool;
        private EcsPool<CoinsCounterChange> m_coinsCounterChangePool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinsCounterFilter = m_world.Filter<CoinsCounter>().End(1);
            m_coinsCounterChangeFilter = m_world.Filter<CoinsCounterChange>().End();
            
            m_coinsCounterPool = m_world.GetPool<CoinsCounter>();
            m_coinsCounterChangePool = m_world.GetPool<CoinsCounterChange>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var coinsCounterEntity in m_coinsCounterFilter)
            foreach (var coinsCounterChange in m_coinsCounterChangeFilter)
            {
                m_coinsCounterPool.Get(coinsCounterEntity).Count += m_coinsCounterChangePool.Get(coinsCounterChange).CorrectionValue;
            }
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (var coinsCounterChange in m_coinsCounterChangeFilter)
            {
                m_world.DelEntity(coinsCounterChange);
            }
        }
    }
}