// using Gameplay.Services.AnimationService;
// using Gameplay.Services.VibrationService;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GameServicesInstaller: IInstaller
  {
    public void Install(IContainerBuilder builder)
    {
      // builder.Register<IAnimationService, AnimationService>(Lifetime.Scoped);
    }
  }
}