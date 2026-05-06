using System.Collections.Generic;
using Application.ContainerMediator;
using Application.StateMachine.Interfaces;
using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Services.LoadScreenService;
using Project.Scripts.Gameplay.Services.ReactionService;
using UniRx;

namespace Application.StateMachine.States
{  
  public sealed class ApplicationState : IApplicationState
  {
    private readonly IReactionService m_reactionService;
    private readonly ILoadScreenService m_loadScreenService;
    
    private readonly IDependenciesContainer m_dependenciesContainer;
    private readonly IApplicationStateMachine m_applicationStateMachine;
    
    private IEnumerable<IEcsSystem> m_ecsSystems;

    private EcsWorld m_world;
    private IEcsSystems m_systems;
    
    private CompositeDisposable m_disposables;

    public ApplicationState(IDependenciesContainer dependenciesContainer, IReactionService reactionService, IApplicationStateMachine applicationStateMachine,
      ILoadScreenService loadScreenService)
    {
      m_reactionService = reactionService;
      m_loadScreenService = loadScreenService;
      m_dependenciesContainer = dependenciesContainer;
      m_applicationStateMachine = applicationStateMachine;
    }

    public async void Enter()
    {    
      await m_dependenciesContainer.CreateApplicationStateDependencies();
      m_ecsSystems = m_dependenciesContainer.ResolveSystems();
      
      m_disposables = new CompositeDisposable();
      
      LaunchEcs();
      Observable.EveryUpdate().Subscribe(_ => Update()).AddTo(m_disposables);
      
      m_loadScreenService.SetComplete();
    }

    public void Exit()
    {
      m_disposables.Dispose();
      DestroyEcs();

      m_dependenciesContainer.CleanupApplicationStateDependencies();
    }

    private void LaunchEcs()
    {
      m_world = new EcsWorld();

      m_systems = new EcsSystems(m_world);
    
      foreach (IEcsSystem ecsSystem in m_ecsSystems)
        m_systems.Add(ecsSystem);
      
      m_systems
#if UNITY_EDITOR
        .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
        .Init();

      m_reactionService.OnRestartGame += Restart;
    }

    private void Update() => 
      m_systems?.Run();

    private void DestroyEcs()
    {
      CleanupSystems();
      CleanupWorlds();
    }

    private void Restart()
    {
      // DOTween.KillAll();
      m_reactionService.OnRestartGame -= Restart;
      m_loadScreenService.ShowLoadScreen(() =>
      {
        m_applicationStateMachine.Enter<RestartGameState>();
      });
    }

    private void CleanupSystems()
    {
      if (m_systems == null)
        return;

      m_systems.Destroy();
      m_systems = null;
    }

    private void CleanupWorlds()
    {
      if (m_world == null)
        return;

      m_world.Destroy();
      m_world = null;
    }
  }
}