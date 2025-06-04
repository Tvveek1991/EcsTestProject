using UnityEngine;

namespace Project.Scripts.Gameplay.Sensors
{
    public class Sensor : MonoBehaviour
    {
        private bool m_isConnected;
        private float m_disableTimer;
        
        public bool IsConnected => m_disableTimer <= 0 && m_isConnected;

        private void OnEnable()
        {
            m_isConnected = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            m_isConnected = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            m_isConnected = false;
        }

        public void SubtractTimer()
        {
            m_disableTimer -= Time.deltaTime;
        }
        
        public void Disable(float duration)
        {
            m_disableTimer = duration;
        }
    }
}