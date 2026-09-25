using System;
using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    [DisallowMultipleComponent]
    public sealed class EntityLink : MonoBehaviour
    {
        private const int UnlinkedEntity = -1;

        private int m_entity = UnlinkedEntity;
        private Guid m_sessionId;

        public int Entity => m_entity;

        public bool IsLinked => m_entity != UnlinkedEntity;

        internal bool IsLinkedTo(Guid sessionId, int entity) =>
            m_sessionId == sessionId && m_entity == entity;

        internal void Bind(Guid sessionId, int entity)
        {
            if (entity < 0)
                throw new ArgumentOutOfRangeException(nameof(entity));

            if (IsLinked && !IsLinkedTo(sessionId, entity))
                throw new InvalidOperationException($"{name} is already linked to another entity or session.");

            m_sessionId = sessionId;
            m_entity = entity;
        }

        internal void Unbind(Guid sessionId, int entity)
        {
            if (!IsLinkedTo(sessionId, entity))
                return;

            m_sessionId = default;
            m_entity = UnlinkedEntity;
        }

        private void OnDestroy()
        {
            m_sessionId = default;
            m_entity = UnlinkedEntity;
        }
    }
}
