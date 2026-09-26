using System;
using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.EntityViewRegistry
{
    public sealed class EntityViewRegistry : IEntityViewRegistry, IDisposable
    {
        private readonly Dictionary<int, EntityView> m_views = new();
        private readonly Dictionary<Collider2D, int> m_colliderEntities = new();
        private readonly Dictionary<int, HashSet<Collider2D>> m_entityColliders = new();
        private readonly Guid m_sessionId = Guid.NewGuid();

        public void Register<TView>(int entity, TView view) where TView : EntityView
        {
            if (entity < 0)
                throw new ArgumentOutOfRangeException(nameof(entity));

            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (m_views.TryGetValue(entity, out EntityView registeredView))
            {
                if (registeredView == view && view.Link.IsLinkedTo(m_sessionId, entity))
                    return;

                throw new InvalidOperationException($"Entity {entity} already has a registered view.");
            }

            view.Link.Bind(m_sessionId, entity);
            m_views.Add(entity, view);
        }

        public void RegisterCollider(int entity, Collider2D collider)
        {
            if (collider == null)
                throw new ArgumentNullException(nameof(collider));

            if (!m_views.TryGetValue(entity, out EntityView view) ||
                view == null ||
                !view.Link.IsLinkedTo(m_sessionId, entity))
                throw new InvalidOperationException($"Entity {entity} must have an active view before registering a collider.");

            if (m_colliderEntities.TryGetValue(collider, out int registeredEntity))
            {
                if (registeredEntity == entity)
                    return;

                throw new InvalidOperationException($"Collider {collider.name} is already linked to entity {registeredEntity}.");
            }

            m_colliderEntities.Add(collider, entity);

            if (!m_entityColliders.TryGetValue(entity, out HashSet<Collider2D> colliders))
            {
                colliders = new HashSet<Collider2D>();
                m_entityColliders.Add(entity, colliders);
            }

            colliders.Add(collider);
        }

        public bool TryGet<TView>(int entity, out TView view) where TView : EntityView
        {
            if (!m_views.TryGetValue(entity, out EntityView registeredView))
            {
                view = null;
                return false;
            }

            if (registeredView == null || !registeredView.Link.IsLinkedTo(m_sessionId, entity))
            {
                m_views.Remove(entity);
                UnregisterColliders(entity);
                view = null;
                return false;
            }

            view = registeredView as TView;
            return view != null;
        }

        public bool TryGetEntity(EntityView view, out int entity)
        {
            if (view == null || !view.Link.IsLinked)
            {
                entity = default;
                return false;
            }

            entity = view.Link.Entity;

            return m_views.TryGetValue(entity, out EntityView registeredView) &&
                   registeredView == view &&
                   view.Link.IsLinkedTo(m_sessionId, entity);
        }

        public bool TryGetEntity(Collider2D collider, out int entity)
        {
            if (collider == null || !m_colliderEntities.TryGetValue(collider, out entity))
            {
                entity = default;
                return false;
            }

            if (m_views.TryGetValue(entity, out EntityView view) &&
                view != null &&
                view.Link.IsLinkedTo(m_sessionId, entity))
                return true;

            RemoveCollider(collider, entity);
            entity = default;
            return false;
        }

        public bool Unregister(int entity)
        {
            if (!m_views.TryGetValue(entity, out EntityView view))
                return false;

            m_views.Remove(entity);
            UnregisterColliders(entity);

            if (view != null)
                view.Link.Unbind(m_sessionId, entity);

            return true;
        }

        public int Count<TView>() where TView : EntityView
        {
            int count = 0;

            foreach (KeyValuePair<int, EntityView> pair in m_views)
            {
                if (pair.Value is TView && pair.Value.Link.IsLinkedTo(m_sessionId, pair.Key))
                    count++;
            }

            return count;
        }

        public void Clear()
        {
            foreach (KeyValuePair<int, EntityView> pair in m_views)
            {
                if (pair.Value != null)
                    pair.Value.Link.Unbind(m_sessionId, pair.Key);
            }

            m_views.Clear();
            m_colliderEntities.Clear();
            m_entityColliders.Clear();
        }

        public void Dispose() => Clear();

        private void UnregisterColliders(int entity)
        {
            if (!m_entityColliders.TryGetValue(entity, out HashSet<Collider2D> colliders))
                return;

            foreach (Collider2D collider in colliders)
                m_colliderEntities.Remove(collider);

            m_entityColliders.Remove(entity);
        }

        private void RemoveCollider(Collider2D collider, int entity)
        {
            m_colliderEntities.Remove(collider);

            if (!m_entityColliders.TryGetValue(entity, out HashSet<Collider2D> colliders))
                return;

            colliders.Remove(collider);

            if (colliders.Count == 0)
                m_entityColliders.Remove(entity);
        }
    }
}
