using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CoinsService;
using Project.Scripts.Gameplay.Services.GameLevelService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string CoinViewsParentName = "CoinViewsContainer";

        private readonly CoinView m_coinViewPrefab;
        private readonly ICoinsService m_coinsService;
        private readonly IGameLevelService m_gameLevelService;

        private EcsWorld m_world;

        private EcsFilter m_coinViewFilter;
        private EcsFilter m_gameLevelViewRefsFilter;

        private EcsPool<TransformKeeper> m_transformPool;

        private GameObject m_parentObject;
        private List<Transform> m_coinSpawnPoints;

        public CoinsViewInitSystem(CoinView coinViewPrefab, ICoinsService coinsService, IGameLevelService gameLevelService)
        {
            m_coinsService = coinsService;
            m_coinViewPrefab = coinViewPrefab;
            m_gameLevelService = gameLevelService;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_coinViewFilter = m_world.Filter<CoinViewKeeper>().Inc<TransformKeeper>().End();
            
            m_transformPool = m_world.GetPool<TransformKeeper>();

            CreateCoinViews();

            SetCoinViewsStartPosition();
        }
        
        public void Destroy(IEcsSystems systems)
        {
            m_coinsService.Clear();
            Object.Destroy(m_parentObject);
        }

        private void CreateCoinViews()
        {
            m_parentObject = new GameObject(CoinViewsParentName);
            m_coinSpawnPoints = m_gameLevelService.View.GetCoinsSpawnPoints();

            for (int i = 0; i < m_coinSpawnPoints.Count; i++)
            {
                var newEntity = m_world.NewEntity();
                var coinView = Object.Instantiate(m_coinViewPrefab, m_parentObject.transform).GetComponent<CoinView>();

                m_coinsService.AddCoinView(newEntity, coinView);
                
                AttachComponents(newEntity, coinView);
            }
            
            m_coinsService.RefreshTotalCount();
        }

        private void AttachComponents(int entityIndex, CoinView coinView)
        {
            m_world.GetPool<CoinViewKeeper>().Add(entityIndex);
            
            AttachTransformComponent();

            void AttachTransformComponent()
            {
                ref TransformKeeper transformKeeper = ref m_world.GetPool<TransformKeeper>().Add(entityIndex);
                transformKeeper.ObjectTransform = coinView.transform;
            }
        }

        private void SetCoinViewsStartPosition()
        {
            var listSpawnPoints = m_coinSpawnPoints;
            foreach (var coinView in m_coinViewFilter)
            {
                if (listSpawnPoints.Count <= 0) continue;

                m_transformPool.Get(coinView).ObjectTransform.position = listSpawnPoints[0].position;
                listSpawnPoints.RemoveAt(0);
            }
        }
    }
}