using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.TutorialService;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CreateTutorialViewSystem : IEcsInitSystem
    {
        private readonly ICanvasService m_canvasService;
        private readonly ITutorialService m_tutorialService;
        private readonly GameObject m_tutorialPrefab;
        
        private EcsWorld m_world;
        
        private EcsPool<Tutorial> m_tutorialViewPool;
        
        public CreateTutorialViewSystem(TextMeshProUGUI tutorialPrefab, ICanvasService canvasService, ITutorialService tutorialService)
        {
            m_canvasService = canvasService;
            m_tutorialService = tutorialService;
            m_tutorialPrefab = tutorialPrefab.gameObject;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_tutorialViewPool = m_world.GetPool<Tutorial>();

            CreateTutorialView();
        }

        private void CreateTutorialView()
        {
            var newEntity = m_world.NewEntity();
            m_tutorialViewPool.Add(newEntity);
            
            var spawnPoint = m_canvasService.Canvas.transform;
            var view = Object.Instantiate(m_tutorialPrefab, spawnPoint);
            
            m_tutorialService.Construct(newEntity, view);
        }
    }
}