namespace Application.StateMachine.Interfaces
{
  public interface IApplicationExitableState
  {
    void Exit();
  }
  
  public interface IApplicationState : IApplicationExitableState
  {
    void Enter();
  }
}