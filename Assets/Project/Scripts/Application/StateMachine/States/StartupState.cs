using Application.StateMachine.Interfaces;
// using Gameplay.Services.VibrationService;

namespace Application.StateMachine.States
{  
  public sealed class StartupState : IApplicationState
  {
    private readonly IApplicationStateMachine _applicationStateMachine;
    // private readonly IVibrationService _vibrationService;

    public StartupState(/*IVibrationService vibrationService, */IApplicationStateMachine applicationStateMachine)
    {
      // _vibrationService = vibrationService;
      _applicationStateMachine = applicationStateMachine;
    }

    public void Enter()
    {
      // _vibrationService.Initialize();
      _applicationStateMachine.Enter<ApplicationState>();
    }

    public void Exit()
    { }
  }
}