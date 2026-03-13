using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.FinishViewService
{
    public class FinishViewService : IFinishViewService
    {
        private int m_entity;
        private FinishView m_view;

        public int Entity => m_entity;
        public FinishView View => m_view;
        
        public void Construct(int entity, FinishView view)
        {
            m_entity = entity;
            m_view = view;
        }
    }
}
