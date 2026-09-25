using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.EntityViewRegistry
{
    public interface IEntityViewRegistry
    {
        void Register<TView>(int entity, TView view) where TView : EntityView;
        bool TryGet<TView>(int entity, out TView view) where TView : EntityView;
        bool TryGetEntity(EntityView view, out int entity);
        bool Unregister(int entity);
        int Count<TView>() where TView : EntityView;
        void Clear();
    }
}
