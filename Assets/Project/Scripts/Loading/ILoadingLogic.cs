using System;

namespace _Project.Scripts.Features.Loading
{
  public interface ILoadingLogic
  {
    void Enter(Action onEnd);
    void Exit();
  }
}