using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCounterViewSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly CoinsCounterView m_coinsCounterViewPrefab;
        
        private EcsWorld m_world;

        private EcsFilter m_canvasFilter;
        private EcsFilter m_coinsCounterFilter;
        private EcsFilter m_coinsCounterViewRefFilter;

        private EcsPool<CanvasComponent> m_canvasPool;
        private EcsPool<CoinsCounterComponent> m_coinsCounterPool;
        private EcsPool<CoinsCounterViewRefComponent> m_coinsCounterViewPool;

        private GameObject m_parentObject;
        
        public CoinsCounterViewSystem(CoinsCounterView coinsCounterViewPrefab)
        {
            m_coinsCounterViewPrefab = coinsCounterViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_canvasFilter = m_world.Filter<CanvasComponent>().End();
            m_coinsCounterFilter = m_world.Filter<CoinsCounterComponent>().End();
            m_coinsCounterViewRefFilter = m_world.Filter<CoinsCounterViewRefComponent>().End();
            
            m_canvasPool = m_world.GetPool<CanvasComponent>();
            m_coinsCounterPool = m_world.GetPool<CoinsCounterComponent>();
            m_coinsCounterViewPool = m_world.GetPool<CoinsCounterViewRefComponent>();
            
            CreateCoinsCounterView();
        }
        
        public void Run(IEcsSystems systems)
        {
            RefreshCoinsCounterView();
        }

        public void Destroy(IEcsSystems systems) =>
            Object.Destroy(m_parentObject);
        
        private void CreateCoinsCounterView()
        {
            foreach (var canvasEntity in m_canvasFilter)
            {
                var newEntity = m_world.NewEntity();
                var spawnPoint = m_canvasPool.Get(canvasEntity).Canvas.transform;
                var view = Object.Instantiate(m_coinsCounterViewPrefab, spawnPoint).GetComponent<CoinsCounterView>();

                AttachCoinsCounterViewComponent(newEntity, view);
            }
            
            void AttachCoinsCounterViewComponent(int newEntity, CoinsCounterView coinsCounterView)
            {
                ref CoinsCounterViewRefComponent view = ref m_coinsCounterViewPool.Add(newEntity);
                view.CoinsCounterView = coinsCounterView;
            }
        }
        
        private void RefreshCoinsCounterView()
        {
            foreach (var coinsCounterEntity in m_coinsCounterFilter)
            foreach (var coinsCounterViewRefEntity in m_coinsCounterViewRefFilter)
            {
                var newValue = m_coinsCounterPool.Get(coinsCounterEntity).Count;
                m_coinsCounterViewPool.Get(coinsCounterViewRefEntity).CoinsCounterView.SetCount(newValue);
            }
        }
    }
}