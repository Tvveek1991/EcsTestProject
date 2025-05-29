using UnityEngine;

namespace Project.Scripts.Gameplay.Sensors
{
    public class Sensor : MonoBehaviour
    {
        private bool m_isGrounded;
        private float m_disableTimer;
        
        public bool IsGrounded => m_disableTimer <= 0 && m_isGrounded;

        private void OnEnable()
        {
            m_isGrounded = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            m_isGrounded = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            m_isGrounded = false;
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