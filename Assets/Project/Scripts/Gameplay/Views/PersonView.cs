using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class PersonView : EntityView
    {
        [SerializeField] private Transform m_healthSpawnPoint;
        [SerializeField] private Transform m_checkerSpawnPoint;

        public Transform GetHealthFollowPoint() =>
            m_healthSpawnPoint;
        
        public Transform GetCheckerSpawnPoint() =>
            m_checkerSpawnPoint;
    }
}
