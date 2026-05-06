using System;

namespace Project.Scripts.Gameplay.Services.ReactionService
{
    public interface IReactionService
    {
        event Action OnRestartGame;

        void RestartGame();
    }
}
