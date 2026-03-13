using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.CoinsCounterService
{
    public class CoinsCounterService : ICoinsCounterService
    {
        private int m_entity;
        private CoinsCounterView m_view;

        public int Entity => m_entity;
        public CoinsCounterView View => m_view;
        
        public void Construct(int entity, CoinsCounterView view)
        {
            m_entity = entity;
            m_view = view;
        }
    }
}
