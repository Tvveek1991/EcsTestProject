using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class GameLevelView : MonoBehaviour
    {
        [SerializeField] private Transform m_heroSpawnPoint;
        [SerializeField] private List<Transform> m_boxSpawnPoints;
        [SerializeField] private List<Transform> m_coinsSpawnPoints;

        public Vector3 GetHeroSpawnPoint() =>
            m_heroSpawnPoint.position;
        
        public List<Transform> GetBoxSpawnPoints() =>
            m_boxSpawnPoints;
        
        public List<Transform> GetCoinsSpawnPoints() =>
            m_coinsSpawnPoints;
    }
}