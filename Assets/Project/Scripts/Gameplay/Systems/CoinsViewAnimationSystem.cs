using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewAnimationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_animatedCoinViewFilter;
        
        private EcsPool<TransformComponent> m_transformPool;
        private EcsPool<CoinsCounterChangeComponent> m_coinsCounterChangePool;
        private EcsPool<CoinViewFlyAwayAnimationComponent> m_coinViewFlyAwayAnimationPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
        
            m_animatedCoinViewFilter = m_world.Filter<CoinViewRefComponent>().Inc<CoinViewFlyAwayAnimationComponent>().Inc<TransformComponent>().End();

            m_transformPool = m_world.GetPool<TransformComponent>();
            m_coinsCounterChangePool = m_world.GetPool<CoinsCounterChangeComponent>();
            m_coinViewFlyAwayAnimationPool = m_world.GetPool<CoinViewFlyAwayAnimationComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var coinView in m_animatedCoinViewFilter)
            {
                var coinTransform = m_transformPool.Get(coinView).ObjectTransform;
                
                m_coinViewFlyAwayAnimationPool.Del(coinView);
                
                var sequence = DOTween.Sequence();
                sequence
                    .Append(coinTransform.DOScale(0, .5f))
                    .Join(coinTransform.DOLocalMoveY(coinTransform.localPosition.y + 1, .5f))
                    .OnComplete(() =>
                    {
                        var entity = m_world.NewEntity();
                        m_coinsCounterChangePool.Add(entity).CorrectionValue = 1;

                        coinTransform.DOKill();
                        
                        sequence.Kill();
                        sequence = null;
                    
                        // m_transformPool.Del(coinView);
                        // m_transformPool.Del(coinView);
                        m_world.DelEntity(coinView);
                        
                        Object.Destroy(coinTransform.gameObject);
                    });
            }
        }
    }
}