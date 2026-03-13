using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.CoinsCounterService;
using Project.Scripts.Gameplay.Services.CoinsService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCounterViewChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly CoinsCounterView m_coinsCounterViewPrefab;
        private readonly ICoinsCounterService m_coinsCounterService;
        private readonly ICoinsService m_coinsService;

        private EcsWorld m_world;

        private EcsFilter m_coinsCounterFilter;

        private EcsPool<CoinsCounter> m_coinsCounterPool;

        public CoinsCounterViewChangeSystem(ICoinsCounterService coinsCounterService, ICoinsService coinsService)
        {
            m_coinsService = coinsService;
            m_coinsCounterService = coinsCounterService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinsCounterFilter = m_world.Filter<CoinsCounter>().End(1);
            
            m_coinsCounterPool = m_world.GetPool<CoinsCounter>();
        }

        public void Run(IEcsSystems systems)
        {
            RefreshCoinsCounterView();
        }
        
        private void RefreshCoinsCounterView()
        {
            foreach (var coinsCounterEntity in m_coinsCounterFilter)
            {
                if(m_coinsCounterService.View == null)
                    continue;
                
                var newValue = m_coinsCounterPool.Get(coinsCounterEntity).Count;
                SetCount(newValue);
            }
        }

        private void SetCount(int score) => 
            m_coinsCounterService.View.ScoreText.text = $"{score}/{m_coinsService.TotalCount}";
    }
}