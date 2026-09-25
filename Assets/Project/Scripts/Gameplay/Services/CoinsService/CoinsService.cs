using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsService
{
    public class CoinsService : ICoinsService
    {
        private readonly IEntityViewRegistry m_entityViewRegistry;
        private int m_totalCount;

        public CoinsService(IEntityViewRegistry entityViewRegistry)
        {
            m_entityViewRegistry = entityViewRegistry;
        }

        public int TotalCount => m_totalCount;

        public void RefreshTotalCount() => m_totalCount = m_entityViewRegistry.Count<CoinView>();
    }
}
