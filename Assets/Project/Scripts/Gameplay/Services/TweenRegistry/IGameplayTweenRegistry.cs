using System;
using DG.Tweening;
using Project.Scripts.Gameplay.Ecs;

namespace Project.Scripts.Gameplay.Services.TweenRegistry
{
    public interface IGameplayTweenRegistry : IGameSessionOperation
    {
        bool IsSessionActive { get; }
        T Track<T>(T tween) where T : Tween;
        bool TryExecute(Action callback);
    }
}
