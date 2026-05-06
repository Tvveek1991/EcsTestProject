using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingMono : MonoBehaviour
  {
    private const float TIME = .5f;
    private const float FADE_DURATION = .25f;

    public CanvasGroup CanvasGroup;

    private bool m_isComplete;

    [SerializeField] private List<Transform> fieldSymbol;

    public void Construct(Action onLoad, Action onEnd) =>
      Animate(onLoad, onEnd);

    public void SetComplete() =>
      m_isComplete = true;
    
    private void Animate(Action onLoad, Action onEnd)
    {
      CanvasGroup.alpha = 0;
      CanvasGroup.DOFade(1, FADE_DURATION).SetDelay(0.2f)
        .OnComplete(() => onLoad?.Invoke());

      var startValue = 0f;
      DOTween.To(() => startValue, x => startValue = x, 1f, TIME)
        .SetLoops(Random.Range(2, 8), LoopType.Restart)
        .SetEase(Ease.Linear)
        .OnStart(Animate)
        .OnStepComplete(Animate)
        .OnComplete(() => Hide(onEnd));
    }

    private async void Hide(Action onEnd)
    {
      while (!m_isComplete) await Awaitable.NextFrameAsync();
      
      fieldSymbol.ForEach(item => item.DOKill());
      
      CanvasGroup.DOFade(0, FADE_DURATION / 2f)
        .OnComplete(() => onEnd?.Invoke());
    }

    private void Animate()
    {
      while (true)
      {
        int rnd = Random.Range(0, fieldSymbol.Count);
        if (DOTween.IsTweening(fieldSymbol[rnd]))
          continue;
        
        if(m_isComplete)
          break;

        fieldSymbol[rnd]
          .DOScale(Vector3.one, .5f)
          .SetEase(Ease.Linear)
          .SetLoops(2, LoopType.Yoyo)
          .OnComplete(() => { fieldSymbol[rnd].DOKill(); });
        break;
      }
    }
  }
}