using System.Collections.Generic;
using Application.ContainerMediator;
using Application.StateMachine.Interfaces;
using Leopotam.EcsLite;
using UniRx;

namespace Application.StateMachine.States
{  
  public sealed class ApplicationState : IApplicationState
  {
    private readonly IDependenciesContainer _dependenciesContainer;
    private IEnumerable<IEcsSystem> _ecsSystems;

    private EcsWorld _world;
    private IEcsSystems _systems;

    private CompositeDisposable _disposables;

    public ApplicationState(IDependenciesContainer dependenciesContainer) => 
      _dependenciesContainer = dependenciesContainer;

    public async void Enter()
    {    
      await _dependenciesContainer.CreateApplicationStateDependencies();
      _ecsSystems = _dependenciesContainer.ResolveSystems();
      
      _disposables = new CompositeDisposable();
      
      LaunchEcs();
      Observable.EveryUpdate().Subscribe(_ => Update()).AddTo(_disposables);
    }

    public void Exit()
    {
      _disposables.Dispose();
      DestroyEcs();
      
      _dependenciesContainer.CleanupApplicationStateDependencies();
    }

    private void LaunchEcs()
    {
      _world = new EcsWorld();

      _systems = new EcsSystems(_world);
    
      foreach (IEcsSystem ecsSystem in _ecsSystems)
        _systems.Add(ecsSystem);
      
      _systems
#if UNITY_EDITOR
        .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
        .Init();
    }

    private void Update() => 
      _systems?.Run();

    private void DestroyEcs()
    {
      CleanupSystems();
      CleanupWorlds();
    }

    private void CleanupSystems()
    {
      if (_systems == null)
        return;

      _systems.Destroy();
      _systems = null;
    }

    private void CleanupWorlds()
    {
      if (_world == null)
        return;

      _world.Destroy();
      _world = null;
    }
  }
}