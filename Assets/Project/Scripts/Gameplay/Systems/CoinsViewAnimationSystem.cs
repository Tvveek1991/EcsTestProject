using System.Collections.Generic;
using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CoinsService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewAnimationSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem, IEcsPostRunSystem
    {
        private readonly ICoinsService m_coinsService;
        private readonly List<Sequence> m_sequences = new();
        
        private EcsWorld m_world;

        private EcsFilter m_animatedCoinViewFilter;
        
        private EcsPool<TransformKeeper> m_transformPool;
        private EcsPool<CoinsCounterChange> m_coinsCounterChangePool;

        public CoinsViewAnimationSystem(ICoinsService coinsService)
        {
            m_coinsService = coinsService;
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

                var sequence = DOTween.Sequence();
                sequence
                    .Append(coinTransform.DOScale(0, .5f))
                    .Join(coinTransform.DOLocalMoveY(coinTransform.localPosition.y + 1, .5f))
                    .OnComplete(() =>
                    {
                        var entity = m_world.NewEntity();
                        m_coinsCounterChangePool.Add(entity).CorrectionValue = 1;
                        coinTransform.DOKill();

                        m_sequences.Remove(sequence);
                        
                        sequence.Kill();
                        sequence = null;

                        Object.Destroy(m_coinsService.GetViewByEntity(coinView).gameObject);
                        m_coinsService.RemoveView(coinView);
                    });
                m_sequences.Add(sequence);
            }
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (var coinView in m_animatedCoinViewFilter)
            {
                m_world.DelEntity(coinView);
            }
        }
        
        public void Destroy(IEcsSystems systems)
        {
            m_sequences.ForEach(item =>
            {
                item?.Kill();
            });
        }
    }
}