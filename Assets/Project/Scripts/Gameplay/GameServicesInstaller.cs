using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.GameLevelService;
using Project.Scripts.Gameplay.Services.TutorialService;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameServicesInstaller: IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      builder.Register<ICanvasService, CanvasService>(Lifetime.Scoped);
      builder.Register<IGameLevelService, GameLevelService>(Lifetime.Scoped);
      builder.Register<ITutorialService, TutorialService>(Lifetime.Scoped);
    }
  }
}