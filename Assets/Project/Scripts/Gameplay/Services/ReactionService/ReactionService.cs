using System;

namespace Project.Scripts.Gameplay.Services.ReactionService
{
    public class ReactionService : IReactionService
    {
        public event Action OnRestartGame;
        
        public void RestartGame()
        {
            OnRestartGame?.Invoke();
        }
    }
}
