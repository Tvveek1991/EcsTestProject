using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Services.GameLevelService
{
    public interface IGameLevelService
    {
        int Entity { get; }
        GameLevelView View { get; }

        void Construct(int entity, GameLevelView view);
    }
}
