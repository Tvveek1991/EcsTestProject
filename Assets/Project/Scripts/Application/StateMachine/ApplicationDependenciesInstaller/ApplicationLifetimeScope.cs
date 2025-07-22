using Application.ContainerMediator;
using Application.StateMachine.Interfaces;
using Application.StateMachine.States;
using AssetProvider;
using Project.Scripts.Application;
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

      RegisterEntryPoint(builder);
    }

    private void RegisterEntryPoint(IContainerBuilder builder)
    {
      builder.RegisterEntryPoint<Bootstrapper>();
      
      builder.Register<IApplicationStateMachine, ApplicationStateMachine>(Lifetime.Singleton);
      
      builder.Register<ApplicationState>(Lifetime.Singleton);
      builder.Register<StartupState>(Lifetime.Singleton);
      builder.Register<EmptyState>(Lifetime.Singleton);
    }
  }
}