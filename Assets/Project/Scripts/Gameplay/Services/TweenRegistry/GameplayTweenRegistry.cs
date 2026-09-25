using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Project.Scripts.Gameplay.Services.TweenRegistry
{
    public sealed class GameplayTweenRegistry : IGameplayTweenRegistry, IDisposable
    {
        private readonly HashSet<Tween> m_tweens = new();

        public bool IsSessionActive { get; private set; } = true;

        public T Track<T>(T tween) where T : Tween
        {
            if (tween == null)
                throw new ArgumentNullException(nameof(tween));

            if (!IsSessionActive)
            {
                tween.Kill();
                return tween;
            }

            m_tweens.Add(tween);
            TweenCallback onKill = tween.onKill;
            tween.OnKill(() =>
            {
                onKill?.Invoke();
                m_tweens.Remove(tween);
            });

            return tween;
        }

        public bool TryExecute(Action callback)
        {
            if (!IsSessionActive || callback == null)
                return false;

            callback();
            return true;
        }

        public void Cancel()
        {
            if (!IsSessionActive)
                return;

            IsSessionActive = false;

            while (m_tweens.Count > 0)
            {
                Tween tween = null;

                foreach (Tween trackedTween in m_tweens)
                {
                    tween = trackedTween;
                    break;
                }

                if (tween == null)
                    break;

                tween.Kill();
                m_tweens.Remove(tween);
            }
        }

        public void Dispose() =>
            Cancel();
    }
}
