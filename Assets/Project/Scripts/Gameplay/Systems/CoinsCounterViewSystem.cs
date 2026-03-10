using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCounterViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly CoinsCounterView m_coinsCounterViewPrefab;
        
        private EcsWorld m_world;

        private EcsFilter m_canvasFilter;
        private EcsFilter m_coinsCounterFilter;
        private EcsFilter m_coinViewRefFilter;
        private EcsFilter m_coinsCounterViewRefFilter;

        private EcsPool<CanvasKeeper> m_canvasPool;
        private EcsPool<CoinsCounter> m_coinsCounterPool;
        private EcsPool<CoinsCounterViewRef> m_coinsCounterViewPool;

        private int m_coinsTotalCount;
        
        public CoinsCounterViewSystem(CoinsCounterView coinsCounterViewPrefab)
        {
            m_coinsCounterViewPrefab = coinsCounterViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_canvasFilter = m_world.Filter<CanvasKeeper>().End(1);
            m_coinViewRefFilter = m_world.Filter<CoinViewRef>().End();
            m_coinsCounterFilter = m_world.Filter<CoinsCounter>().End(1);
            m_coinsCounterViewRefFilter = m_world.Filter<CoinsCounterViewRef>().End(1);
            
            m_canvasPool = m_world.GetPool<CanvasKeeper>();
            m_coinsCounterPool = m_world.GetPool<CoinsCounter>();
            m_coinsCounterViewPool = m_world.GetPool<CoinsCounterViewRef>();
            
            m_coinsTotalCount = m_coinViewRefFilter.GetEntitiesCount();
            
            CreateCoinsCounterView();
        }

        public void Run(IEcsSystems systems)
        {
            RefreshCoinsCounterView();
        }

        private void CreateCoinsCounterView()
        {
            foreach (var canvasEntity in m_canvasFilter)
            {
                var newEntity = m_world.NewEntity();
                var spawnPoint = m_canvasPool.Get(canvasEntity).Canvas.transform;
                var view = Object.Instantiate(m_coinsCounterViewPrefab, spawnPoint).GetComponent<CoinsCounterView>();
                view.ScoreText.text = "0";

                AttachCoinsCounterViewComponent(newEntity, view);
            }
            
            void AttachCoinsCounterViewComponent(int newEntity, CoinsCounterView coinsCounterView)
            {
                ref CoinsCounterViewRef view = ref m_coinsCounterViewPool.Add(newEntity);
                view.CoinsCounterView = coinsCounterView;
            }
        }
        
        private void RefreshCoinsCounterView()
        {
            foreach (var coinsCounterEntity in m_coinsCounterFilter)
            foreach (var coinsCounterViewRefEntity in m_coinsCounterViewRefFilter)
            {
                var newValue = m_coinsCounterPool.Get(coinsCounterEntity).Count;
                SetCount(coinsCounterViewRefEntity, newValue);
            }
        }

        private void SetCount(int entity, int score) => 
            m_coinsCounterViewPool.Get(entity).CoinsCounterView.ScoreText.text = $"{score}/{m_coinsTotalCount}";
    }
}