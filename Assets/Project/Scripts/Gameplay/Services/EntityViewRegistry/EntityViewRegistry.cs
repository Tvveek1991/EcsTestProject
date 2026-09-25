using System;
using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.EntityViewRegistry
{
    public sealed class EntityViewRegistry : IEntityViewRegistry, IDisposable
    {
        private readonly Dictionary<int, EntityView> m_views = new();
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

        public bool Unregister(int entity)
        {
            if (!m_views.TryGetValue(entity, out EntityView view))
                return false;

            m_views.Remove(entity);

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
        }

        public void Dispose() => Clear();
    }
}
