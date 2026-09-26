using Leopotam.EcsLite;
using Project.Scripts.Gameplay;
using Project.Scripts.Gameplay.Ecs;
using Project.Scripts.Gameplay.Systems;
using Project.Scripts.Gameplay.Systems.Input;
using Project.Scripts.Gameplay.Systems.PersonAnimations;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameSystemsInstaller : IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      builder.Register<GameSystemsComposer>(Lifetime.Scoped);

      RegisterInitializationSystems(builder);
      RegisterInputSystems(builder);
      RegisterSimulationSystems(builder);
      RegisterPhysicsSystems(builder);
      RegisterPresentationSystems(builder);
      RegisterCleanupSystems(builder);
    }

    private void RegisterInitializationSystems(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, CanvasInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateGameLevelViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CameraInitSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, PlayerInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, BoxViewInitSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, CoinsCounterInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsCounterViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CreateTutorialViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonConnectSensorsInitSystem>(Lifetime.Scoped);
    }

    private void RegisterInputSystems(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, InputSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputJumpSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputRollSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputMoveSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputHurtSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputAttackSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckInputBlockSystem>(Lifetime.Scoped);
    }

    private void RegisterSimulationSystems(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, ReactionSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, HealthInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsCounterChangeSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsViewCheckSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, AttackSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CheckHitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HealthChangeSystem>(Lifetime.Scoped);
    }

    private void RegisterPhysicsSystems(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, JumpSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, BlockSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, RunSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, RollingSystem>(Lifetime.Scoped);
    }

    private void RegisterPresentationSystems(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, CameraFollowSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsCounterViewChangeSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, CoinsViewAnimationSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, HealthViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HealthViewFollowSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, HealthViewChangeSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, FlipHeroViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonMoveAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonFallingAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonJumpAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonRollingAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonBlockAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonAttackAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonSlidingAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonHurtAnimatorSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, PersonDeadAnimatorSystem>(Lifetime.Scoped);

      builder.Register<IEcsSystem, CheckDeathSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, FinishViewInitSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, EndGameSystem>(Lifetime.Scoped);
    }

    private void RegisterCleanupSystems(IContainerBuilder builder)
    {
      builder.Register<IEcsSystem, DestroyHealthViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, DestroyObjectViewSystem>(Lifetime.Scoped);
      builder.Register<IEcsSystem, EndOfFrameCleanupSystem>(Lifetime.Scoped);
    }
  }
}
