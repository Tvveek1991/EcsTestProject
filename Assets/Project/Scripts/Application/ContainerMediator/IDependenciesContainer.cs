using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;

namespace Application.ContainerMediator
{  
  public interface IDependenciesContainer
  {
    UniTask CreateApplicationStateDependencies();
    IEnumerable<IEcsSystem> ResolveSystems();
    void CleanupApplicationStateDependencies();
  }
}