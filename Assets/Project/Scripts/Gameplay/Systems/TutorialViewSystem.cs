using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class TutorialViewSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly ICanvasService m_canvasService;
        private readonly GameObject m_tutorialPrefab;
        
        private EcsWorld m_world;

        private EcsFilter m_tutorialViewRefFilter;
        
        private EcsPool<TutorialViewRef> m_tutorialViewPool;
        
        public TutorialViewSystem(TextMeshProUGUI tutorialPrefab, ICanvasService canvasService)
        {
            m_canvasService = canvasService;
            m_tutorialPrefab = tutorialPrefab.gameObject;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_tutorialViewRefFilter = m_world.Filter<TutorialViewRef>().End(1);

            m_tutorialViewPool = m_world.GetPool<TutorialViewRef>();

            CreateTutorialView();
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (var entity in m_tutorialViewRefFilter)
            {
                Object.Destroy(m_tutorialViewPool.Get(entity).TutorialView.gameObject);
            }
        }

        private void CreateTutorialView()
        {
            var newEntity = m_world.NewEntity();
            var spawnPoint = m_canvasService.Canvas.transform;
            var view = Object.Instantiate(m_tutorialPrefab, spawnPoint);
                
            AttachTutorialViewComponent();
            
            void AttachTutorialViewComponent()
            {
                ref TutorialViewRef component = ref m_tutorialViewPool.Add(newEntity);
                component.TutorialView = view;
            }
            
        }
    }
}