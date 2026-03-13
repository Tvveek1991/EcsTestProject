using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.FinishViewService
{
    public interface IFinishViewService
    {
        int Entity { get; }
        FinishView View { get; }

        void Construct(int entity, FinishView view);
    }
}