using System.Collections.Generic;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsService
{
    public class CoinsService : ICoinsService
    {
        private readonly Dictionary<int, CoinView> m_views = new();
        
        private int m_totalCount;

        public int TotalCount => m_totalCount;
        public Dictionary<int, CoinView> Views => m_views;
        
        
        public void RefreshTotalCount() => m_totalCount = m_views.Count;

        public void AddCoinView(int entity, CoinView view) => m_views.Add(entity, view);
        
        public void RemoveView(int entity) => m_views.Remove(entity);

        public void Clear() => m_views.Clear();
    }
}
