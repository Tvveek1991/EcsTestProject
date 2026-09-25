using System.Collections.Generic;
using System;
using System.Threading;
using Application.StateMachine.ApplicationDependenciesInstaller;
using AssetProvider.Scripts;
using Cysharp.Threading.Tasks;
using Gameplay;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Ecs;
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

    public async UniTask CreateApplicationStateDependencies(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var gamePlayInstaller = new GamePlayInstaller(_assetProvider);
      var gameSystemsInstaller = new GameSystemsInstaller();
      var gameServicesInstaller = new GameServicesInstaller();

      LifetimeScope applicationScope = null;

      try
      {
        await gamePlayInstaller.Preload(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        applicationScope = _applicationLifetimeScope.CreateChild(builder =>
        {
          gamePlayInstaller.Install(builder);
          gameServicesInstaller.Install(builder);
          gameSystemsInstaller.Install(builder);
        });

        cancellationToken.ThrowIfCancellationRequested();

        _gamePlayInstaller = gamePlayInstaller;
        _applicationScope = applicationScope;
      }
      catch
      {
        applicationScope?.Dispose();
        gamePlayInstaller.Clear();
        throw;
      }
    }

    public IEnumerable<IEcsSystem> ResolveSystems()
    {
      var registeredSystems = _applicationScope.Container.Resolve<IEnumerable<IEcsSystem>>();
      var composer = _applicationScope.Container.Resolve<GameSystemsComposer>();

      return composer.Compose(registeredSystems);
    }

    public void CleanupApplicationStateDependencies()
    {
      _applicationScope?.Dispose();
      _applicationScope = null;

      _gamePlayInstaller?.Clear();
      _gamePlayInstaller = null;
    }
  }
}
