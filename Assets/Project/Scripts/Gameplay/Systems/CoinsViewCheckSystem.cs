using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CoinsService;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewCheckSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly ICoinsService m_coinsService;
        
        private EcsWorld m_world;

        private EcsFilter m_coinViewFilter;

        private EcsPool<CoinViewFlyAwayAnimation> m_coinViewFlyAwayAnimationPool;

        public CoinsViewCheckSystem(ICoinsService coinsService)
        {
            m_coinsService = coinsService;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinViewFilter = m_world.Filter<CoinViewKeeper>().End();
            
            m_coinViewFlyAwayAnimationPool = m_world.GetPool<CoinViewFlyAwayAnimation>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var coinView in m_coinViewFilter)
            {
                if(!m_coinsService.Views.TryGetValue(coinView, out var view))
                    continue;
                
                if (view.Sensor.IsConnected)
                {
                    m_coinViewFlyAwayAnimationPool.Add(coinView);
                    
                    view.Sensor.Disable(float.MaxValue);
                }
            }
        }
    }
}
