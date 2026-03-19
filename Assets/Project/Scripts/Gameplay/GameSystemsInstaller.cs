using Leopotam.EcsLite;
using Project.Scripts.Gameplay;
using Project.Scripts.Gameplay.Systems;
using Project.Scripts.Gameplay.Systems.Input;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameSystemsInstaller : IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, CanvasInitSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, CreateGameLevelViewSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CameraInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CameraFollowSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CoinsCounterInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsCounterChangeSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsCounterViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsCounterViewChangeSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CoinsViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsViewCheckSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsViewAnimationSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CreateTutorialViewSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, PlayerInitSystem>(Lifetime.Scoped);////////

      builder.Register<IEcsSystem, InputSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputJumpSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputRollSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputMoveSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputHurtSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputAttackSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputBlockSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, BoxViewInitSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, HealthInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HealthChangeSystem>(Lifetime.Scoped); 
      
      builder.Register<IEcsSystem, HealthViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HealthViewFollowSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HealthViewChangeSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, PersonConnectSensorsInitSystem>(Lifetime.Scoped);////
      
      builder.Register<IEcsSystem, FlipHeroViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, JumpSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, BlockSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, RunSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, AttackSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, RollingSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, CheckHitSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, PersonAnimatorSystem>(Lifetime.Scoped);////

      builder.Register<IEcsSystem, CheckDeathSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, DestroyHealthViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, DestroyBoxViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, DestroyJumpSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, FinishViewInitSystem>(Lifetime.Scoped);
      
      builder.Register<IEcsSystem, EndGameSystem>(Lifetime.Scoped);
    }
  }
}