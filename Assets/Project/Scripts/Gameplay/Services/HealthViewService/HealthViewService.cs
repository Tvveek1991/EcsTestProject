using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.HealthViewService
{
    public class HealthViewService : IHealthViewService
    {
        private readonly Dictionary<int, HealthView> m_views = new();
        
        public Dictionary<int, HealthView> Views => m_views;

        
        
        public void AddHealthView(int entity, HealthView view) => m_views.Add(entity, view);
        
        public void RemoveView(int entity) => m_views.Remove(entity);

        public void Clear() => m_views.Clear();
    }
}
