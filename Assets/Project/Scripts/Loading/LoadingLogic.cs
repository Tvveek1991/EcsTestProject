using System;
using AssetProvider.Scripts;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingLogic : ILoadingLogic
  {
    private readonly ILoadingFactory m_loadingFactory;

    public LoadingLogic(IAssetProvider assetProvider) => 
      m_loadingFactory = new LoadingFactory(assetProvider);

    public void Enter(Action onEnd) => 
      m_loadingFactory.CreateScreen(() =>
      {
        onEnd?.Invoke();
      });

    public void SetComplete()
    {
      if(m_loadingFactory.LoadingMono != null)
        m_loadingFactory.LoadingMono.SetComplete();
    }

    public void Exit() => 
      m_loadingFactory.ClearScreen();
  }
}