using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewCheckSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_coinViewRefFilter;
        private EcsFilter m_gameLevelViewRefsFilter;

        private EcsPool<CoinViewRefComponent> m_coinViewRefPool;
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;
        private EcsPool<CoinsCounterChangeComponent> m_coinsCounterChangePool;


        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinViewRefFilter = m_world.Filter<CoinViewRefComponent>().End();
            
            m_coinViewRefPool = m_world.GetPool<CoinViewRefComponent>();
            m_coinsCounterChangePool = m_world.GetPool<CoinsCounterChangeComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var coinView in m_coinViewRefFilter)
            {
                if (m_coinViewRefPool.Get(coinView).CoinView.Sensor.IsConnected)
                {
                    var entity = m_world.NewEntity();
                    m_coinsCounterChangePool.Add(entity).CorrectionValue = 1;
                    
                    m_coinViewRefPool.Get(coinView).CoinView.Sensor.Disable(float.MaxValue);

                    ref var view = ref m_coinViewRefPool.Get(coinView);
                    Object.Destroy(view.CoinView.gameObject);
                    
                    m_world.DelEntity(coinView);
                }
                
            }
        }
    }
}
