using System;
using _Project.Scripts.Features.Loading;
using AssetProvider.Scripts;

namespace Project.Scripts.Gameplay.Services.LoadScreenService
{
    public class LoadScreenService : ILoadScreenService
    {
        private readonly LoadingLogic m_loadingLogic;
        
        public LoadScreenService(IAssetProvider assetProvider)
        {
            m_loadingLogic = new LoadingLogic(assetProvider);
        }

        public void ShowLoadScreen(Action onComplete)
        {
            m_loadingLogic?.Enter(onComplete);
        }

        public void SetComplete()
        {
            m_loadingLogic?.SetComplete();
        }
    }
}
