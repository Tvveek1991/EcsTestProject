using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsService
{
    public interface ICoinsService
    {
        void AddCoinView(int entity, CoinView view);
        CoinView GetViewByEntity(int entity);
        void Clear();
    }
}
