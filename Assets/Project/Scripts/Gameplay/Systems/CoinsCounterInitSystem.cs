using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCounterInitSystem : IEcsInitSystem
    {
        private EcsWorld m_world;

        private EcsPool<CoinsCounter> m_coinsCounterPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinsCounterPool = m_world.GetPool<CoinsCounter>();

            CreateCoinsCounter();
        }

        private void CreateCoinsCounter()
        {
            var entity = m_world.NewEntity();
            m_coinsCounterPool.Add(entity).Count = 0;
        }
    }
}
