using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class FinishViewInitSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private const float FADE_DURATION = .5f;
        
        private readonly FinishView m_finishViewPrefab;

        private EcsWorld m_world;

        private EcsFilter m_canvasFilter;
        private EcsFilter m_deadPlayerFilter;
        private EcsFilter m_finishViewRefFilter;

        private EcsPool<CanvasComponent> m_canvasPool;
        private EcsPool<FinishViewRefComponent> m_finishViewRefPool;

        private FinishView m_finishView;

        public FinishViewInitSystem(FinishView finishViewPrefab)
        {
            m_finishViewPrefab = finishViewPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_canvasFilter = m_world.Filter<CanvasComponent>().End();
            m_finishViewRefFilter = m_world.Filter<FinishViewRefComponent>().End();
            m_deadPlayerFilter = m_world.Filter<PlayerComponent>().Inc<DeadComponent>().End();

            m_canvasPool = m_world.GetPool<CanvasComponent>();
            m_finishViewRefPool = m_world.GetPool<FinishViewRefComponent>();
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
            if(m_deadPlayerFilter.GetEntitiesCount() == 0)
                return;
            
            foreach (var canvasEntity in m_canvasFilter)
            {
                if (m_finishViewRefFilter.GetEntitiesCount() > 0)
                    return;

                var newEntityIndex = m_world.NewEntity();
                var spawnPoint = m_canvasPool.Get(canvasEntity).Canvas.transform;

                m_finishView = Object.Instantiate(m_finishViewPrefab, spawnPoint).GetComponent<FinishView>();

                AttachViewToGameLevelViewReference();

                ShowView();

                void AttachViewToGameLevelViewReference()
                {
                    ref FinishViewRefComponent cellViewRef = ref m_finishViewRefPool.Add(newEntityIndex);
                    cellViewRef.FinishView = m_finishView;
                }
            }
        }

        private void ShowView()
        {
            m_finishView.CanvasGroup.alpha = 0;
            m_finishView.CanvasGroup.DOFade(1f, FADE_DURATION);

            m_finishView.Title.text = "You died";
        }
    }
}