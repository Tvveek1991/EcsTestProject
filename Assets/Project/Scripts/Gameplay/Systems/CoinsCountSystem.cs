using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCountSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_coinsCounterFilter;
        private EcsFilter m_coinsCounterChangeFilter;

        private EcsPool<CoinsCounterComponent> m_coinsCounterPool;
        private EcsPool<CoinsCounterChangeComponent> m_coinsCounterChangePool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinsCounterFilter = m_world.Filter<CoinsCounterComponent>().End();
            m_coinsCounterChangeFilter = m_world.Filter<CoinsCounterChangeComponent>().End();
            
            m_coinsCounterPool = m_world.GetPool<CoinsCounterComponent>();
            m_coinsCounterChangePool = m_world.GetPool<CoinsCounterChangeComponent>();

            CreateCoinsCounterComponent();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var coinsCounterEntity in m_coinsCounterFilter)
            foreach (var coinsCounterChange in m_coinsCounterChangeFilter)
            {
                m_coinsCounterPool.Get(coinsCounterEntity).Count += m_coinsCounterChangePool.Get(coinsCounterChange).CorrectionValue;
                m_coinsCounterChangePool.Del(coinsCounterChange);
            }
        }

        private void CreateCoinsCounterComponent()
        {
            var entity = m_world.NewEntity();
            m_coinsCounterPool.Add(entity).Count = 0;
        }
    }
}
