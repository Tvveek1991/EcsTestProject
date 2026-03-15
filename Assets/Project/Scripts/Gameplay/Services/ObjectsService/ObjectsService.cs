using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Gameplay.Services.ObjectsService
{
    public class ObjectsService : IObjectsService
    {
        private readonly Dictionary<int, ObjectView> m_views = new();
        
        public Dictionary<int, ObjectView> Views => m_views;

        
        
        public void AddObjectView(int entity, ObjectView view) => m_views.Add(entity, view);
        
        public void RemoveView(int entity) => m_views.Remove(entity);

        public void Clear() => m_views.Clear();
    }
}