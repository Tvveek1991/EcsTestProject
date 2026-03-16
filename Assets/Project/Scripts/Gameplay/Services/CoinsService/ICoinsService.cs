using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsService
{
    public interface ICoinsService
    {
        int TotalCount { get; }
        Dictionary<int, CoinView> Views { get; }
        
        void RefreshTotalCount();
        void AddCoinView(int entity, CoinView view);
        void RemoveView(int entity);
        void Clear();
    }
}
