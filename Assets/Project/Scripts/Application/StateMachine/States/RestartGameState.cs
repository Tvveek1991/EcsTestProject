using Application.StateMachine.Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application.StateMachine.States
{
  [UsedImplicitly]
  public sealed class RestartGameState : IApplicationState
  {  
    public void Enter() => 
      SceneManager.LoadSceneAsync("Main");
    
    public void Exit() => 
      Debug.Log("Exit RestartGameState");
  }
}