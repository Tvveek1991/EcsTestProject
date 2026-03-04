using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.Gameplay.Systems
{
    public class FinishViewInitSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private const float FADE_DURATION = .5f;
        
        private readonly FinishView m_finishViewPrefab;

        private EcsWorld m_world;

        private EcsFilter m_canvasFilter;
        private EcsFilter m_endGameFilter;
        private EcsFilter m_deadPlayerFilter;
        private EcsFilter m_coinViewRefFilter;
        private EcsFilter m_coinsCounterFilter;

        private EcsPool<CanvasComponent> m_canvasPool;
        private EcsPool<CoinsCounterComponent> m_coinsCounterPool;
        private EcsPool<FinishViewRefComponent> m_finishViewRefPool;
        private EcsPool<EndGameComponent> m_endGamePool;

        private FinishView m_finishView;
        
        private int m_coinsTotalCount;

        public FinishViewInitSystem(FinishView finishViewPrefab)
        {
            m_finishViewPrefab = finishViewPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_canvasFilter = m_world.Filter<CanvasComponent>().End();
            m_endGameFilter = m_world.Filter<EndGameComponent>().End();
            m_coinViewRefFilter = m_world.Filter<CoinViewRefComponent>().End();
            m_coinsCounterFilter = m_world.Filter<CoinsCounterComponent>().End();
            m_deadPlayerFilter = m_world.Filter<PlayerComponent>().Inc<DeadComponent>().End();

            m_canvasPool = m_world.GetPool<CanvasComponent>();
            m_endGamePool = m_world.GetPool<EndGameComponent>();
            m_coinsCounterPool = m_world.GetPool<CoinsCounterComponent>();
            m_finishViewRefPool = m_world.GetPool<FinishViewRefComponent>();

            m_coinsTotalCount = m_coinViewRefFilter.GetEntitiesCount();
        }

        public void Run(IEcsSystems systems)
        {
            TryCreateView();
        }

        public void Destroy(IEcsSystems systems)
        {
            if (m_finishView.gameObject)
                Object.Destroy(m_finishView.gameObject);
        }

        private void TryCreateView()
        {
            if(m_coinsCounterFilter.GetEntitiesCount() == 0)
                return;

            int currentCount = m_coinsCounterPool.Get(m_coinsCounterFilter.GetRawEntities()[0]).Count;

            bool isEndGame = currentCount >= m_coinsTotalCount || m_deadPlayerFilter.GetEntitiesCount() != 0;
            
            if(!isEndGame)
                return;
            
            bool isWin = currentCount >= m_coinsTotalCount;
            
            foreach (var canvasEntity in m_canvasFilter)
            {
                if (m_endGameFilter.GetEntitiesCount() > 0)
                    return;
                
                var endGameEntity = m_world.NewEntity();
                m_endGamePool.Add(endGameEntity);

                var newEntityIndex = m_world.NewEntity();
                var spawnPoint = m_canvasPool.Get(canvasEntity).Canvas.transform;

                m_finishView = Object.Instantiate(m_finishViewPrefab, spawnPoint);

                AddListeners();

                AttachViewToGameLevelViewReference();

                ShowView(isWin);

                void AttachViewToGameLevelViewReference()
                {
                    ref FinishViewRefComponent cellViewRef = ref m_finishViewRefPool.Add(newEntityIndex);
                    cellViewRef.FinishView = m_finishView;
                }
            }
        }

        private void AddListeners()
        {
            m_finishView.RestartButton.onClick.AddListener(() =>
            {
                m_world.Destroy();
                SceneManager.LoadSceneAsync("Main");
            });
        }

        private void ShowView(bool isWin)
        {
            m_finishView.CanvasGroup.alpha = 1;
            m_finishView.CanvasGroup.DOFade(1f, FADE_DURATION);

            m_finishView.Title.text = isWin ? "You win" : "You died";
            m_finishView.ButtonText.text = isWin ? "Start new game" : "Restart";
        }
    }
}