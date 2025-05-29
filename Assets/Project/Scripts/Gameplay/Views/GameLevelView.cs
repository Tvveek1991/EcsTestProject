using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class GameLevelView : MonoBehaviour
    {
        [SerializeField] private Transform m_heroSpawnPoint;

        public Vector3 GetHeroSpawnPoint() =>
            m_heroSpawnPoint.position;
    }
}