using System;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Gameplay.Sensors
{
    public class Sensor : MonoBehaviour
    {
        private const float DISABLE_DELAY = 0.1f;
        
        private bool m_isConnected;
        private float m_disableTimer;
        
        private Tween m_disconnectTween;
        
        public bool IsConnected => m_disableTimer <= 0 && m_isConnected;

        private void OnDestroy()
        {
            m_disconnectTween?.Kill();
            m_disconnectTween = null;
        }

        private void OnEnable()
        {
            m_isConnected = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            m_isConnected = true;
            m_disconnectTween?.Kill();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            m_disconnectTween = DOVirtual.DelayedCall(DISABLE_DELAY, () => m_isConnected = false);
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