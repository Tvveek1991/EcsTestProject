using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsService
{
    public interface ICoinsService
    {
        int TotalCount { get; }
        void RefreshTotalCount();
        void AddCoinView(int entity, CoinView view);
        CoinView GetViewByEntity(int entity);
        void RemoveView(int entity);
        void Clear();
    }
}
