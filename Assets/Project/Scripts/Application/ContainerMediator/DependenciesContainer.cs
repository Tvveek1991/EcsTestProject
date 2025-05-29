using System.Collections.Generic;
using Application.StateMachine.ApplicationDependenciesInstaller;
using AssetProvider.Scripts;
using Cysharp.Threading.Tasks;
using Gameplay;
using Leopotam.EcsLite;
using VContainer;
using VContainer.Unity;

namespace Application.ContainerMediator
{
  public class DependenciesContainer : IDependenciesContainer
  {
    private readonly ApplicationLifetimeScope _applicationLifetimeScope;

    private readonly IAssetProvider _assetProvider;
    
    private LifetimeScope _applicationScope;
    private GamePlayInstaller _gamePlayInstaller;

    public DependenciesContainer(ApplicationLifetimeScope applicationLifetimeScope, IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
      _applicationLifetimeScope = applicationLifetimeScope;
    }

    public async UniTask CreateApplicationStateDependencies()
    {       
      _gamePlayInstaller = new GamePlayInstaller(_assetProvider);
      var gameSystemsInstaller = new GameSystemsInstaller();
      var gameServicesInstaller = new GameServicesInstaller();
      
      await _gamePlayInstaller.Preload();
      
      _applicationScope = _applicationLifetimeScope.CreateChild(builder =>
      {
        _gamePlayInstaller.Install(builder);
        gameServicesInstaller.Install(builder);
        gameSystemsInstaller.Install(builder);
      });
    }

    public IEnumerable<IEcsSystem> ResolveSystems() => 
      _applicationScope.Container.Resolve<IEnumerable<IEcsSystem>>();

    public void CleanupApplicationStateDependencies()
    {
      _gamePlayInstaller.Clear();
      
      _applicationScope.Dispose();
      _applicationScope = null;
    }
  }
}