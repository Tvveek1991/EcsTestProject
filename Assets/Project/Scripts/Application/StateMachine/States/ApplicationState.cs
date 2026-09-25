using System;
using System.Collections.Generic;
using System.Threading;
using Application.ContainerMediator;
using Application.StateMachine.Interfaces;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Ecs;
using Project.Scripts.Gameplay.Services.LoadScreenService;
using Project.Scripts.Gameplay.Services.ReactionService;
using UnityEngine;
using VContainer.Unity;

namespace Application.StateMachine.States
{  
  public sealed class ApplicationState : IApplicationState, ITickable
  {
    private readonly IReactionService m_reactionService;
    private readonly ILoadScreenService m_loadScreenService;
    
    private readonly IDependenciesContainer m_dependenciesContainer;
    private readonly IApplicationStateMachine m_applicationStateMachine;
    
    private IEnumerable<IEcsSystem> m_ecsSystems;

    private GameEcsLoop m_gameEcsLoop;

    private CancellationTokenSource m_initializationCancellationTokenSource;
    
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
      CancelInitialization();

      var cancellationTokenSource = new CancellationTokenSource();
      m_initializationCancellationTokenSource = cancellationTokenSource;

      try
      {
        await m_dependenciesContainer.CreateApplicationStateDependencies(cancellationTokenSource.Token);

        if (cancellationTokenSource.IsCancellationRequested)
        {
          m_dependenciesContainer.CleanupApplicationStateDependencies();
          return;
        }

        m_ecsSystems = m_dependenciesContainer.ResolveSystems();
        LaunchEcs();

        m_loadScreenService.SetComplete();
      }
      catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
      {
      }
      catch (Exception exception)
      {
        m_dependenciesContainer.CleanupApplicationStateDependencies();
        Debug.LogException(exception);
      }
      finally
      {
        if (m_initializationCancellationTokenSource == cancellationTokenSource)
          m_initializationCancellationTokenSource = null;

        cancellationTokenSource.Dispose();
      }
    }

    public void Exit()
    {
      m_reactionService.OnRestartGame -= Restart;
      CancelInitialization();
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

    public void Tick() =>
      m_gameEcsLoop?.Tick();

    private void DestroyEcs()
    {
      m_gameEcsLoop?.Stop();
      m_gameEcsLoop = null;
      m_ecsSystems = null;
    }

    private void CancelInitialization()
    {
      if (m_initializationCancellationTokenSource == null)
        return;

      m_initializationCancellationTokenSource.Cancel();
      m_initializationCancellationTokenSource = null;
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
