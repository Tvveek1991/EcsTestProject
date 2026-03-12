using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.GameLevelService;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameServicesInstaller: IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      // builder.Register<IAnimationService, AnimationService>(Lifetime.Scoped);
      builder.Register<ICanvasService, CanvasService>(Lifetime.Scoped);
      builder.Register<IGameLevelService, GameLevelService>(Lifetime.Scoped);
    }
  }
}