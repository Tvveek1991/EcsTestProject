using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public abstract class EntityView : MonoBehaviour
    {
        private EntityLink m_link;

        public EntityLink Link
        {
            get
            {
                if (m_link != null)
                    return m_link;

                m_link = GetComponent<EntityLink>();

                if (m_link == null)
                    m_link = gameObject.AddComponent<EntityLink>();

                return m_link;
            }
        }
    }
}
