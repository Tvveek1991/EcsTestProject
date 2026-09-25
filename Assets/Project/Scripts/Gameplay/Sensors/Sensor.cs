using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Scripts.Gameplay.Sensors
{
    public class Sensor : MonoBehaviour
    {
        private const float DISABLE_DELAY = 0.1f;
        
        private readonly HashSet<Collider2D> m_connectedColliders = new();
        private CancellationTokenSource m_pendingExitCancellationTokenSource;
        private bool m_isExitPending;
        private float m_disableTimer;

        public bool IsConnected => m_disableTimer <= 0 && (m_connectedColliders.Count > 0 || m_isExitPending);

        private void OnEnable()
        {
            CancelPendingExit();
            m_connectedColliders.Clear();
            m_isExitPending = false;
            m_disableTimer = 0;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null)
                return;

            CancelPendingExit();
            m_isExitPending = false;
            m_connectedColliders.Add(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == null || !m_connectedColliders.Remove(other) || m_connectedColliders.Count > 0)
                return;

            ScheduleDisconnect();
        }

        private void OnDisable()
        {
            CancelPendingExit();
            m_connectedColliders.Clear();
            m_isExitPending = false;
        }

        private void OnDestroy()
        {
            CancelPendingExit();
            m_connectedColliders.Clear();
            m_isExitPending = false;
        }

        private void ScheduleDisconnect()
        {
            CancelPendingExit();
            m_isExitPending = true;

            var cancellationTokenSource = new CancellationTokenSource();
            m_pendingExitCancellationTokenSource = cancellationTokenSource;
            DisconnectAfterDelay(cancellationTokenSource).Forget();
        }

        private async UniTaskVoid DisconnectAfterDelay(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(DISABLE_DELAY), cancellationToken: cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }
            finally
            {
                if (m_pendingExitCancellationTokenSource == cancellationTokenSource)
                {
                    m_pendingExitCancellationTokenSource = null;
                    m_isExitPending = false;
                }

                cancellationTokenSource.Dispose();
            }
        }

        private void CancelPendingExit()
        {
            if (m_pendingExitCancellationTokenSource == null)
                return;

            var cancellationTokenSource = m_pendingExitCancellationTokenSource;
            m_pendingExitCancellationTokenSource = null;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        public void SubtractTimer()
        {
            m_disableTimer = Mathf.Max(0, m_disableTimer - Time.deltaTime);
        }
        
        public void Disable(float duration)
        {
            m_disableTimer = duration;
        }
    }
}
