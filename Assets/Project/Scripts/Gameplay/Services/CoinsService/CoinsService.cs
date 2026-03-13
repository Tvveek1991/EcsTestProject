using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsService
{
    public class CoinsService : ICoinsService
    {
        private readonly Dictionary<int, CoinView> m_views = new();
        
        public void AddCoinView(int entity, CoinView view) => m_views.Add(entity, view);

        public CoinView GetViewByEntity(int entity) => m_views[entity];

        public void Clear() => m_views.Clear();
    }
}
