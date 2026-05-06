using System;

namespace Project.Scripts.Gameplay.Services.LoadScreenService
{
    public interface ILoadScreenService
    {
        void ShowLoadScreen(Action onComplete);
        void SetComplete();
    }
}
