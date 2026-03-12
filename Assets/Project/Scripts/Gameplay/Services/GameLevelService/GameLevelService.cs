using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.GameLevelService
{
    public sealed class GameLevelService : IGameLevelService
    {
        private int m_entity;
        private GameLevelView m_view;

        public int Entity => m_entity;
        public GameLevelView View => m_view;
        
        public void Construct(int entity, GameLevelView view)
        {
            m_entity = entity;
            m_view = view;
        }
    }
}
