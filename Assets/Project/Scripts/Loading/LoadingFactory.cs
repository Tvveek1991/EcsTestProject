using System;
using _Project.Scripts.Core_Logic.Common;
using AssetProvider.Scripts;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingFactory : GameObjectFactory, ILoadingFactory
  {
    private const string ADDRESS = "LoadingView";
    
    public LoadingMono LoadingMono { get; private set; }

    public LoadingFactory(IAssetProvider assetProvider) : base(assetProvider)
    { }

    public async void CreateScreen(Action onLoad)
    {
      ClearScreen();
      LoadingMono = (await CreateAsync(ADDRESS)).GetComponentInChildren<LoadingMono>();
      LoadingMono.Construct(onLoad, ClearScreen);
    }

    public void ClearScreen()
    {
      if (LoadingMono == null)
        return;
      
      Release(ADDRESS);
      Clear(LoadingMono.gameObject);
      LoadingMono = null;
    }
  }
}