using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Ecs;

namespace Application.ContainerMediator
{  
  public interface IDependenciesContainer
  {
    UniTask CreateApplicationStateDependencies(CancellationToken cancellationToken);
    IEnumerable<IEcsSystem> ResolveSystems();
    IEnumerable<IGameSessionOperation> ResolveSessionOperations();
    void CleanupApplicationStateDependencies();
  }
}
