using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CanvasInitSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly Canvas m_canvasPrefab;
        private readonly ICanvasService m_canvasService;

        private EcsWorld m_world;

        private EcsPool<CanvasKeeper> m_canvasPool;

        public CanvasInitSystem(Canvas canvasPrefab, ICanvasService canvasService)
        {
            m_canvasPrefab = canvasPrefab;
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
            var canvasEntityIndex = m_world.NewEntity();
            m_canvasPool.Add(canvasEntityIndex);
            
            var canvas = Object.Instantiate(m_canvasPrefab);
            m_canvasService.Construct(canvasEntityIndex, canvas);
        }

        public void Destroy(IEcsSystems systems)
        {
            m_canvasService.Clear();
        }
    }
}