using UnityEngine;

namespace Project.Scripts.Gameplay.Services.TutorialService
{
    public class TutorialService : ITutorialService
    {
        private int m_entity;
        private GameObject m_view;

        public int Entity => m_entity;
        public GameObject View => m_view;
        
        public void Construct(int entity, GameObject view)
        {
            m_entity = entity;
            m_view = view;
        }
    }
}
