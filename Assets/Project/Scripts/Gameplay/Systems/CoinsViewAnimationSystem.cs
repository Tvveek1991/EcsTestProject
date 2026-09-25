using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Services.TweenRegistry;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewAnimationSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private readonly IEntityViewRegistry m_entityViewRegistry;
        private readonly IGameplayTweenRegistry m_gameplayTweenRegistry;
        
        private EcsWorld m_world;

        private EcsFilter m_animatedCoinViewFilter;
        
        private EcsPool<TransformKeeper> m_transformPool;
        private EcsPool<CoinsCounterChange> m_coinsCounterChangePool;

        public CoinsViewAnimationSystem(IEntityViewRegistry entityViewRegistry, IGameplayTweenRegistry gameplayTweenRegistry)
        {
            m_entityViewRegistry = entityViewRegistry;
            m_gameplayTweenRegistry = gameplayTweenRegistry;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
        
            m_animatedCoinViewFilter = m_world.Filter<CoinViewKeeper>().Inc<CoinViewFlyAwayAnimation>().Inc<TransformKeeper>().End();

            m_transformPool = m_world.GetPool<TransformKeeper>();
            m_coinsCounterChangePool = m_world.GetPool<CoinsCounterChange>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var coinView in m_animatedCoinViewFilter)
            {
                var coinTransform = m_transformPool.Get(coinView).ObjectTransform;

                var sequence = m_gameplayTweenRegistry.Track(DOTween.Sequence());
                sequence
                    .Append(coinTransform.DOScale(0, .5f))
                    .Join(coinTransform.DOLocalMoveY(coinTransform.localPosition.y + 1, .5f))
                    .OnComplete(() =>
                    {
                        m_gameplayTweenRegistry.TryExecute(() =>
                        {
                            var entity = m_world.NewEntity();
                            m_coinsCounterChangePool.Add(entity).CorrectionValue = 1;
                            coinTransform.DOKill();

                            if (!m_entityViewRegistry.TryGet(coinView, out CoinView view))
                                return;

                            m_entityViewRegistry.Unregister(coinView);
                            Object.Destroy(view.gameObject);
                        });
                    });
            }
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (var coinView in m_animatedCoinViewFilter)
            {
                m_world.DelEntity(coinView);
            }
        }
        
    }
}
