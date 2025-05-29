using Application.StateMachine.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace Application.StateMachine.States
{
  [UsedImplicitly]
  public sealed class EmptyState : IApplicationState
  {  
    public void Enter() => 
      Debug.Log("Enter EmptyState");
    
    public void Exit() => 
      Debug.Log("Exit EmptyState");
  }
}