using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Gameplay.Services.ObjectsService
{
    public interface IObjectsService
    {
        Dictionary<int, ObjectView> Views { get; }
        
        void AddObjectView(int entity, ObjectView view);
        void RemoveView(int entity);
        void Clear();
    }
}