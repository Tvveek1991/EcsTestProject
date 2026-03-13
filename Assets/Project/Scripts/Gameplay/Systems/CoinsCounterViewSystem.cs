using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.CoinsCounterService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCounterViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly CoinsCounterView m_coinsCounterViewPrefab;
        private readonly ICoinsCounterService m_coinsCounterService;
        private readonly ICanvasService m_canvasService;

        private EcsWorld m_world;

        private EcsFilter m_coinsCounterFilter;
        private EcsFilter m_coinViewRefFilter;
        private EcsFilter m_coinsCounterViewFilter;

        private EcsPool<CoinsCounter> m_coinsCounterPool;
        private EcsPool<CoinsCounterViewKeeper> m_coinsCounterViewPool;

        private int m_coinsTotalCount;
        
        public CoinsCounterViewSystem(CoinsCounterView coinsCounterViewPrefab, ICoinsCounterService coinsCounterService, ICanvasService canvasService)
        {
            m_canvasService = canvasService;
            m_coinsCounterService = coinsCounterService;
            m_coinsCounterViewPrefab = coinsCounterViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinViewRefFilter = m_world.Filter<CoinViewKeeper>().End();
            m_coinsCounterFilter = m_world.Filter<CoinsCounter>().End(1);
            m_coinsCounterViewFilter = m_world.Filter<CoinsCounterViewKeeper>().End(1);
            
            m_coinsCounterPool = m_world.GetPool<CoinsCounter>();
            m_coinsCounterViewPool = m_world.GetPool<CoinsCounterViewKeeper>();
            
            m_coinsTotalCount = m_coinViewRefFilter.GetEntitiesCount();
            
            CreateCoinsCounterView();
        }

        public void Run(IEcsSystems systems)
        {
            RefreshCoinsCounterView();
        }

        private void CreateCoinsCounterView()
        {
            var newEntity = m_world.NewEntity();
            m_coinsCounterViewPool.Add(newEntity);
            
            var spawnPoint = m_canvasService.Canvas.transform;
            var view = Object.Instantiate(m_coinsCounterViewPrefab, spawnPoint).GetComponent<CoinsCounterView>();
            view.ScoreText.text = "0";

            m_coinsCounterService.Construct(newEntity, view);
        }
        
        private void RefreshCoinsCounterView()
        {
            foreach (var coinsCounterEntity in m_coinsCounterFilter)
            foreach (var coinsCounterViewRefEntity in m_coinsCounterViewFilter)
            {
                if(coinsCounterViewRefEntity != m_coinsCounterService.Entity)
                    continue;
                
                var newValue = m_coinsCounterPool.Get(coinsCounterEntity).Count;
                SetCount(newValue);
            }
        }

        private void SetCount(int score) => 
            m_coinsCounterService.View.ScoreText.text = $"{score}/{m_coinsTotalCount}";
    }
}