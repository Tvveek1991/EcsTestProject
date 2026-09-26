using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.EntityViewRegistry
{
    public interface IEntityViewRegistry
    {
        void Register<TView>(int entity, TView view) where TView : EntityView;
        void RegisterCollider(int entity, Collider2D collider);
        bool TryGet<TView>(int entity, out TView view) where TView : EntityView;
        bool TryGetEntity(EntityView view, out int entity);
        bool TryGetEntity(Collider2D collider, out int entity);
        bool Unregister(int entity);
        int Count<TView>() where TView : EntityView;
        void Clear();
    }
}
