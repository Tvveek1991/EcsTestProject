using Application.StateMachine.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace Application.StateMachine.States
{
  [UsedImplicitly]
  public sealed class RestartGameState : IApplicationState
  {  
    private readonly IApplicationStateMachine m_applicationStateMachine;
    
    public RestartGameState(IApplicationStateMachine applicationStateMachine)
    {
      m_applicationStateMachine = applicationStateMachine;
    }
    
    public void Enter()
    {
      m_applicationStateMachine.Enter<ApplicationState>();
    }

    public void Exit() => 
      Debug.Log("Exit RestartGameState");
  }
}