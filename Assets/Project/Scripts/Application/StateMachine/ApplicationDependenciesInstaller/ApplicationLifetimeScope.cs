using Application.ContainerMediator;
using Application.StateMachine.Interfaces;
using Application.StateMachine.States;
using AssetProvider;
using Project.Scripts.Application;
using Project.Scripts.Gameplay.Services.LoadScreenService;
using Project.Scripts.Gameplay.Services.ReactionService;
// using Gameplay.Services.VibrationService;
using VContainer;
using VContainer.Unity;

namespace Application.StateMachine.ApplicationDependenciesInstaller
{
  public sealed class ApplicationLifetimeScope : LifetimeScope
  {  
    protected override void Configure(IContainerBuilder builder)
    {
      new AssetProviderInstaller().Install(builder);
      builder.Register<IDependenciesContainer, DependenciesContainer>(Lifetime.Singleton);

      // builder.Register<IVibrationService, VibrationService>(Lifetime.Singleton);
      builder.Register<IReactionService, ReactionService>(Lifetime.Singleton);
      builder.Register<ILoadScreenService, LoadScreenService>(Lifetime.Singleton);

      RegisterEntryPoint(builder);
    }

    private void RegisterEntryPoint(IContainerBuilder builder)
    {
      builder.RegisterEntryPoint<Bootstrapper>();
      
      builder.Register<IApplicationStateMachine, ApplicationStateMachine>(Lifetime.Singleton);
      
      builder.RegisterEntryPoint<ApplicationState>(Lifetime.Singleton).AsSelf();
      builder.Register<StartupState>(Lifetime.Singleton);
      builder.Register<RestartGameState>(Lifetime.Singleton);
    }
  }
}
