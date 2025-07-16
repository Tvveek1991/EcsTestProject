using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class PersonView : MonoBehaviour
    {
        [SerializeField] private Transform m_healthSpawnPoint;
        
        public void SetPosition(Vector3 newPosition) => 
            transform.position = newPosition;
        
        public Transform GetHealthSpawnPoint() =>
            m_healthSpawnPoint;
    }
}