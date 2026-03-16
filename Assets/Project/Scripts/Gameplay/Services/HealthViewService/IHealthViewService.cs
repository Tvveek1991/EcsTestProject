using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.HealthViewService
{
    public interface IHealthViewService
    {
        Dictionary<int, HealthView> Views { get; }
        
        void AddHealthView(int entity, HealthView view);
        void RemoveView(int entity);
        void Clear();
    }
}