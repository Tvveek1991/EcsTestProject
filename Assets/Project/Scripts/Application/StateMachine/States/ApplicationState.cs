using System.Collections.Generic;
using Application.ContainerMediator;
using Application.StateMachine.Interfaces;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Ecs;
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

    private GameEcsLoop m_gameEcsLoop;
    
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
      m_disposables?.Dispose();
      m_disposables = null;

      m_reactionService.OnRestartGame -= Restart;
      DestroyEcs();

      m_dependenciesContainer.CleanupApplicationStateDependencies();
    }

    private void LaunchEcs()
    {
      var systems = new List<IEcsSystem>(m_ecsSystems);

#if UNITY_EDITOR
      systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif

      m_gameEcsLoop = new GameEcsLoop();
      m_gameEcsLoop.Start(systems);

      m_reactionService.OnRestartGame += Restart;
    }

    private void Update() => 
      m_gameEcsLoop?.Tick();

    private void DestroyEcs()
    {
      m_gameEcsLoop?.Dispose();
      m_gameEcsLoop = null;
    }

    private void Restart()
    {
      m_reactionService.OnRestartGame -= Restart;
      m_loadScreenService.ShowLoadScreen(() =>
      {
        m_applicationStateMachine.Enter<RestartGameState>();
      });
    }
  }
}
