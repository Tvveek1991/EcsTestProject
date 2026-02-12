using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewInitSystem : IEcsInitSystem
    {
        private const string CoinViewsParentName = "CoinViewsContainer";
        
        private readonly CoinView m_coinViewPrefab;
        
        private EcsWorld m_world;

        private EcsFilter m_coinViewRefFilter;
        private EcsFilter m_gameLevelViewRefsFilter;

        private EcsPool<TransformComponent> m_transformPool;
        private EcsPool<CoinViewRefComponent> m_coinViewRefPool;
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;
        
        private GameObject m_parentObject;
        private List<Transform> m_coinSpawnPoints;
        public CoinsViewInitSystem(CoinView coinViewPrefab) => 
            m_coinViewPrefab = coinViewPrefab;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_coinViewRefFilter = m_world.Filter<CoinViewRefComponent>().Inc<TransformComponent>().End();
            m_gameLevelViewRefsFilter = m_world.Filter<GameLevelViewRefComponent>().End();
            m_transformPool = m_world.GetPool<TransformComponent>();
            m_gameLevelViewRefsPool = m_world.GetPool<GameLevelViewRefComponent>();

            FindSpawnPoints();
            CreateCoinViews();
            
            SetCoinViewsStartPosition();
        }

        private void CreateCoinViews()
        {
            m_parentObject = new GameObject(CoinViewsParentName);

            for (int i = 0; i < m_coinSpawnPoints.Count; i++)
            {
                var gameLevelEntityIndex = m_world.NewEntity();
                var coinView = Object.Instantiate(m_coinViewPrefab, m_parentObject.transform).GetComponent<CoinView>();
                
                AttachComponents(gameLevelEntityIndex, coinView);
            }
        }

        private void AttachComponents(int entityIndex, CoinView coinView)
        {
            AttachTransformComponent();
            AttachCoinViewRefComponent();
            
            void AttachTransformComponent()
            {
                ref TransformComponent transformComponent = ref m_world.GetPool<TransformComponent>().Add(entityIndex);
                transformComponent.ObjectTransform = coinView.transform;
            }

            void AttachCoinViewRefComponent()
            {
                ref CoinViewRefComponent cellViewRef = ref m_world.GetPool<CoinViewRefComponent>().Add(entityIndex);
                cellViewRef.CoinView = coinView;
            }
        }

        private void FindSpawnPoints()
        {
            foreach (var item in m_gameLevelViewRefsFilter)
            {
                ref GameLevelViewRefComponent gameLevelViewRefComponent = ref m_gameLevelViewRefsPool.Get(item);
                m_coinSpawnPoints = gameLevelViewRefComponent.GameLevelView.GetCoinsSpawnPoints();
            }
        }

        private void SetCoinViewsStartPosition()
        {
            var listSpawnPoints = m_coinSpawnPoints;
            foreach (var coinView in m_coinViewRefFilter)
            {
                if (listSpawnPoints.Count <= 0) continue;
                
                m_transformPool.Get(coinView).ObjectTransform.position = listSpawnPoints[0].position;
                listSpawnPoints.RemoveAt(0);
            }
        }
    }
}