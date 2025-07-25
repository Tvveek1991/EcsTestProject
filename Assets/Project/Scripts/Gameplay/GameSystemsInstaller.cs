using Leopotam.EcsLite;
using Project.Scripts.Gameplay;
using Project.Scripts.Gameplay.Systems;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameSystemsInstaller : IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, InputSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CanvasInitSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, CreateGameLevelViewSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, PlayerInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, EnemyInitSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, HealthInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateHealthViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HealthViewFollowSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CameraResizeInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CameraFollowSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, PersonConnectSensorsInitSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CheckPlayerInputSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, FlipHeroViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, JumpSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, BlockSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, RunSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, AttackSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, RollingSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, PersonAnimatorSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, HealthChangeSystem>(Lifetime.Scoped); 
      builder.Register<IEcsSystem, HealthViewChangeSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckDeathSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, DestroyJumpSystem>(Lifetime.Scoped);
    }
  }
}