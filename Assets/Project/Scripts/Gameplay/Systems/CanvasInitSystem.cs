using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CanvasInitSystem : IEcsInitSystem
    {
        private readonly Canvas m_canvasPrefab;

        private EcsWorld m_world;

        private EcsPool<CanvasComponent> m_canvasPool;

        public CanvasInitSystem(Canvas canvasPrefab)
        {
            m_canvasPrefab = canvasPrefab;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_canvasPool = m_world.GetPool<CanvasComponent>();

            CreateCanvas();
        }

        private void CreateCanvas()
        {
            var canvasEntityIndex = m_world.NewEntity();

            var canvas = Object.Instantiate(m_canvasPrefab).GetComponent<Canvas>();

            AttachCanvasReference();

            void AttachCanvasReference()
            {
                ref CanvasComponent canvasRef = ref m_canvasPool.Add(canvasEntityIndex);
                canvasRef.Canvas = canvas;
            }
        }
    }
}