using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class PersonView : MonoBehaviour
    {
        [SerializeField] private Transform m_healthSpawnPoint;

        public Transform GetHealthFollowPoint() =>
            m_healthSpawnPoint;
    }
}