using AssetProvider.Scripts;
using VContainer;
using VContainer.Unity;

namespace AssetProvider
{  
  public class AssetProviderInstaller: IInstaller
  {    
    public void Install(IContainerBuilder builder) => 
      builder.Register<IAssetProvider, global::AssetProvider.Scripts.AssetProvider>(Lifetime.Singleton);
  }
}