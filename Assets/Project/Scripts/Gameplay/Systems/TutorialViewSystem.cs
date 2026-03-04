using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class TutorialViewSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly GameObject m_tutorialPrefab;
        private EcsWorld m_world;

        private EcsFilter m_canvasFilter;
        private EcsFilter m_tutorialViewRefFilter;
        
        private EcsPool<CanvasComponent> m_canvasPool;
        private EcsPool<TutorialViewRefComponent> m_tutorialViewPool;
        
        public TutorialViewSystem(TextMeshProUGUI tutorialPrefab)
        {
            m_tutorialPrefab = tutorialPrefab.gameObject;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_canvasFilter = m_world.Filter<CanvasComponent>().End();
            m_tutorialViewRefFilter = m_world.Filter<TutorialViewRefComponent>().End();

            m_canvasPool = m_world.GetPool<CanvasComponent>();
            m_tutorialViewPool = m_world.GetPool<TutorialViewRefComponent>();

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
            foreach (var canvasEntity in m_canvasFilter)
            {
                var newEntity = m_world.NewEntity();
                var spawnPoint = m_canvasPool.Get(canvasEntity).Canvas.transform;
                var view = Object.Instantiate(m_tutorialPrefab, spawnPoint);
                
                AttachTutorialViewComponent(newEntity, view);
            }
            
            void AttachTutorialViewComponent(int newEntity, GameObject view)
            {
                ref TutorialViewRefComponent component = ref m_tutorialViewPool.Add(newEntity);
                component.TutorialView = view;
            }
            
        }
    }
}