using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class GameLevelView : MonoBehaviour
    {
        [SerializeField] private Transform m_heroSpawnPoint;
        [SerializeField] private List<Transform> m_enemySpawnPoints;

        public Vector3 GetHeroSpawnPoint() =>
            m_heroSpawnPoint.position;
        
        public List<Transform> GetEnemySpawnPoints() =>
            m_enemySpawnPoints;
    }
}