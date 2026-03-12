using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.GameLevelService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string CoinViewsParentName = "CoinViewsContainer";

        private readonly CoinView m_coinViewPrefab;
        private readonly IGameLevelService m_gameLevelService;

        private EcsWorld m_world;

        private EcsFilter m_coinViewRefFilter;
        private EcsFilter m_gameLevelViewRefsFilter;

        private EcsPool<TransformKeeper> m_transformPool;

        private GameObject m_parentObject;
        private List<Transform> m_coinSpawnPoints;

        public CoinsViewInitSystem(CoinView coinViewPrefab, IGameLevelService gameLevelService)
        {
            m_coinViewPrefab = coinViewPrefab;
            m_gameLevelService = gameLevelService;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_coinViewRefFilter = m_world.Filter<CoinViewRef>().Inc<TransformKeeper>().End();
            
            m_transformPool = m_world.GetPool<TransformKeeper>();

            CreateCoinViews();

            SetCoinViewsStartPosition();
        }
        
        public void Destroy(IEcsSystems systems) => 
            Object.Destroy(m_parentObject);

        private void CreateCoinViews()
        {
            m_parentObject = new GameObject(CoinViewsParentName);
            m_coinSpawnPoints = m_gameLevelService.View.GetCoinsSpawnPoints();

            for (int i = 0; i < m_coinSpawnPoints.Count; i++)
            {
                var gameLevelEntityIndex = m_world.NewEntity();
                var coinView = Object.Instantiate(m_coinViewPrefab, m_parentObject.transform).GetComponent<CoinView>();

                AttachComponents(gameLevelEntityIndex, coinView);
            }
        }

        private void AttachComponents(int entityIndex, CoinView coinView)
        {
            AttachCoinViewRefComponent();
            AttachTransformComponent();

            void AttachCoinViewRefComponent()
            {
                ref CoinViewRef cellViewRef = ref m_world.GetPool<CoinViewRef>().Add(entityIndex);
                cellViewRef.CoinView = coinView;
            }

            void AttachTransformComponent()
            {
                ref TransformKeeper transformKeeper = ref m_world.GetPool<TransformKeeper>().Add(entityIndex);
                transformKeeper.ObjectTransform = coinView.transform;
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