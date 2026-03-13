using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsCounterService
{
    public interface ICoinsCounterService
    {
        int Entity { get; }
        CoinsCounterView View { get; }

        void Construct(int entity, CoinsCounterView view);
    }
}