using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class ObjectView : EntityView
    {
        [SerializeField] private Transform m_healthSpawnPoint;
        [SerializeField] private Transform m_destroyParticlesSpawnPoint;

        public Transform GetHealthFollowPoint() =>
            m_healthSpawnPoint;
        
        public Transform GetDestroyParticlesPoint() =>
            m_destroyParticlesSpawnPoint;
    }
}
