using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.BridgeFactory;
using Project.Scripts.Gameplay.Services.TutorialService;

namespace Project.Scripts.Gameplay.Systems
{
    public class CreateTutorialViewSystem : IEcsInitSystem
    {
        private readonly ICanvasService m_canvasService;
        private readonly ITutorialService m_tutorialService;
        private readonly IGameplayUiBridgeFactory m_gameplayUiBridgeFactory;
        
        private EcsWorld m_world;
        
        private EcsPool<TutorialViewRef> m_tutorialViewPool;
        
        public CreateTutorialViewSystem(IGameplayUiBridgeFactory gameplayUiBridgeFactory, ICanvasService canvasService, ITutorialService tutorialService)
        {
            m_canvasService = canvasService;
            m_tutorialService = tutorialService;
            m_gameplayUiBridgeFactory = gameplayUiBridgeFactory;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_tutorialViewPool = m_world.GetPool<TutorialViewRef>();

            CreateTutorialView();
        }

        private void CreateTutorialView()
        {
            var newEntity = m_world.NewEntity();
            m_tutorialViewPool.Add(newEntity);
            
            var spawnPoint = m_canvasService.Canvas.transform;
            var view = m_gameplayUiBridgeFactory.CreateTutorial(spawnPoint);
            
            m_tutorialService.Construct(newEntity, view);
        }
    }
}
