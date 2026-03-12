using UnityEngine;

namespace Project.Scripts.Gameplay.Services.CanvasService
{
    public sealed class CanvasService : ICanvasService
    {
        private int m_entity;
        private Canvas m_canvas;

        public int Entity => m_entity;
        public Canvas Canvas => m_canvas;
        
        public void Construct(int entity, Canvas canvas)
        {
            m_entity = entity;
            m_canvas = canvas;
        }

        public void Clear()
        {
            if(m_canvas.gameObject)
                Object.Destroy(m_canvas.gameObject);
        }
    }
}
