using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.BridgeFactory;
using Project.Scripts.Gameplay.Services.FinishViewService;
using Project.Scripts.Gameplay.Services.TweenRegistry;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class FinishViewInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float FADE_DURATION = .5f;
        
        private readonly IGameplayUiBridgeFactory m_gameplayUiBridgeFactory;
        private readonly IFinishViewService m_finishViewService;
        private readonly ICanvasService m_canvasService;
        private readonly IGameplayTweenRegistry m_gameplayTweenRegistry;

        private EcsWorld m_world;

        private EcsFilter m_endGameFilter;
        private EcsFilter m_deadPlayerFilter;
        private EcsFilter m_coinViewRefFilter;
        private EcsFilter m_coinsCounterFilter;

        private EcsPool<CoinsCounter> m_coinsCounterPool;
        private EcsPool<FinishViewComponent> m_finishViewRefPool;
        private EcsPool<EndGame> m_endGamePool;
        private EcsPool<ReactionComponent> m_reactionPool;
        
        private int m_coinsTotalCount;

        public FinishViewInitSystem(IGameplayUiBridgeFactory gameplayUiBridgeFactory, IFinishViewService finishViewService, ICanvasService canvasService,
            IGameplayTweenRegistry gameplayTweenRegistry)
        {
            m_canvasService = canvasService;
            m_gameplayUiBridgeFactory = gameplayUiBridgeFactory;
            m_finishViewService = finishViewService;
            m_gameplayTweenRegistry = gameplayTweenRegistry;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_endGameFilter = m_world.Filter<EndGame>().End(1);
            m_coinViewRefFilter = m_world.Filter<CoinViewKeeper>().End();
            m_coinsCounterFilter = m_world.Filter<CoinsCounter>().End(1);
            m_deadPlayerFilter = m_world.Filter<Player>().Inc<Dead>().End(1);

            m_endGamePool = m_world.GetPool<EndGame>();
            m_coinsCounterPool = m_world.GetPool<CoinsCounter>();
            m_reactionPool = m_world.GetPool<ReactionComponent>();
            m_finishViewRefPool = m_world.GetPool<FinishViewComponent>();

            m_coinsTotalCount = m_coinViewRefFilter.GetEntitiesCount();
        }

        public void Run(IEcsSystems systems)
        {
            TryCreateView();
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
            
            if (m_endGameFilter.GetEntitiesCount() > 0)
                return;
                
            var endGameEntity = m_world.NewEntity();
            m_endGamePool.Add(endGameEntity);

            var newEntity = m_world.NewEntity();
            m_finishViewRefPool.Add(newEntity);
                
            var spawnPoint = m_canvasService.Canvas.transform;

            var view = m_gameplayUiBridgeFactory.CreateFinish(spawnPoint);
            m_finishViewService.Construct(newEntity, view);

            AddListeners();

            ShowView(isWin);
        }

        private void AddListeners()
        {
            m_finishViewService.View.RestartButton.onClick.AddListener(() =>
            {
                m_gameplayTweenRegistry.TryExecute(() =>
                {
                    var entity = m_world.NewEntity();
                    m_reactionPool.Add(entity).Type = ReactionType.CompleteState;
                });
            });
        }

        private void ShowView(bool isWin)
        {
            m_finishViewService.View.CanvasGroup.alpha = 1;
            m_gameplayTweenRegistry.Track(m_finishViewService.View.CanvasGroup.DOFade(1f, FADE_DURATION));

            m_finishViewService.View.Title.text = isWin ? "You win" : "You died";
            m_finishViewService.View.ButtonText.text = isWin ? "Start new game" : "Restart";
        }
    }
}
