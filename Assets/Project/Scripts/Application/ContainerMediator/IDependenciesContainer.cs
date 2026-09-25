using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;

namespace Application.ContainerMediator
{  
  public interface IDependenciesContainer
  {
    UniTask CreateApplicationStateDependencies(CancellationToken cancellationToken);
    IEnumerable<IEcsSystem> ResolveSystems();
    void CleanupApplicationStateDependencies();
  }
}
