using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewCheckSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_coinViewRefFilter;

        private EcsPool<CoinViewRefComponent> m_coinViewRefPool;
        private EcsPool<CoinViewFlyAwayAnimationComponent> m_coinViewFlyAwayAnimationPool;


        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinViewRefFilter = m_world.Filter<CoinViewRefComponent>().End();
            
            m_coinViewRefPool = m_world.GetPool<CoinViewRefComponent>();
            m_coinViewFlyAwayAnimationPool = m_world.GetPool<CoinViewFlyAwayAnimationComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var coinView in m_coinViewRefFilter)
            {
                if (m_coinViewRefPool.Get(coinView).CoinView.Sensor.IsConnected)
                {
                    m_coinViewFlyAwayAnimationPool.Add(coinView);
                    
                    m_coinViewRefPool.Get(coinView).CoinView.Sensor.Disable(float.MaxValue);
                }
            }
        }
    }
}
