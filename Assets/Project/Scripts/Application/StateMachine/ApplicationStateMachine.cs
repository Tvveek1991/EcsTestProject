using System;
using System.Collections.Generic;
using Application.StateMachine.Interfaces;
using UnityEngine;
using VContainer;

namespace Application.StateMachine
{    
  public sealed class ApplicationStateMachine : IApplicationStateMachine
  {
    private readonly Dictionary<Type, IApplicationExitableState> _states;
    private readonly IObjectResolver _container;
    
    private IApplicationExitableState _activeApplicationExitableState;

    public ApplicationStateMachine(IObjectResolver container)
    {
      _container = container;
      _states = new Dictionary<Type, IApplicationExitableState>();
    }
    
    public void Enter<TState>() where TState : class, IApplicationState
    {
      LazyCreateState<TState>();

      if (IsApplicationAlreadyInThisState<TState>()) 
        return;

      IApplicationState exitableApplicationState = ChangeState<TState>();
      exitableApplicationState.Enter();
    }
    
    private void LazyCreateState<TState>() where TState : class, IApplicationState
    {
      if (!_states.ContainsKey(typeof(TState)))
      {
        TState state = _container.Resolve<TState>();
        _states.Add(typeof(TState), state);
      }
    }

    private bool IsApplicationAlreadyInThisState<TState>() where TState : class, IApplicationState
    {
      if (_activeApplicationExitableState != null && _activeApplicationExitableState.GetType() == typeof(TState))
      {
        Debug.LogWarning($"Application is already in this state {typeof(TState).Name}");
        return true;
      }

      return false;
    }
    
    private TState ChangeState<TState>() where TState : class, IApplicationExitableState
    {
      _activeApplicationExitableState?.Exit();
      
      TState state = GetState<TState>();
      _activeApplicationExitableState = state;
      return state;
    }
    
    private TState GetState<TState>() where TState : class, IApplicationExitableState =>
      _states[typeof(TState)] as TState;
  }
}