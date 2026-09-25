using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.ViewFactory;

namespace Project.Scripts.Gameplay.Systems
{
    public class CanvasInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly IGameplayViewFactory m_gameplayViewFactory;
        private readonly ICanvasService m_canvasService;

        private EcsWorld m_world;

        private EcsPool<CanvasKeeper> m_canvasPool;

        public CanvasInitSystem(IGameplayViewFactory gameplayViewFactory, ICanvasService canvasService)
        {
            m_gameplayViewFactory = gameplayViewFactory;
            m_canvasService = canvasService;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_canvasPool = m_world.GetPool<CanvasKeeper>();

            CreateCanvas();
        }

        private void CreateCanvas()
        {
            var canvasEntity = m_world.NewEntity();
            m_canvasPool.Add(canvasEntity);
            
            var canvas = m_gameplayViewFactory.CreateCanvas();
            m_canvasService.Construct(canvasEntity, canvas);
        }

        public void Destroy(IEcsSystems systems)
        {
            m_canvasService.Clear();
        }
    }
}
