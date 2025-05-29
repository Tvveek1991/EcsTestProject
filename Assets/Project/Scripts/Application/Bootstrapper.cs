using Application.StateMachine.Interfaces;
using Application.StateMachine.States;
using UnityEngine;
using VContainer;

namespace Application
{
  public sealed class Bootstrapper : MonoBehaviour
  {
    private IApplicationStateMachine _applicationStateMachine;
   
    [Inject]
    public void Construct(IApplicationStateMachine applicationStateMachine) =>
      _applicationStateMachine = applicationStateMachine;

    private void Start()
    {
      UnityEngine.Application.targetFrameRate = 60;
      _applicationStateMachine.Enter<StartupState>();
    }
  }
}