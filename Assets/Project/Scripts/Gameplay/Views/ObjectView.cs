using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class ObjectView : MonoBehaviour
    {
        [SerializeField] private Transform m_healthSpawnPoint;

        public Transform GetHealthFollowPoint() =>
            m_healthSpawnPoint;
    }
}
