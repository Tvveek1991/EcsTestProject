using AssetProvider.Scripts;
using Cysharp.Threading.Tasks;
using Gameplay.Data;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Sensors;
using Project.Scripts.Gameplay.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
  public class GamePlayInstaller : IInstaller
  {
    private const string HeroDataAddress = "Hero Data";
    private const string CameraDataAddress = "Camera Data";
    private const string AnimationDataAddress = "Field Animation Data";
    
    private const string GroundSensorAddress = "GroundSensor";
    private const string HeroViewAddress = "HeroView";
    private const string GameLevelViewAddress = "GameLevelView";
    
    private const string CameraAddress = "Camera";

    private HeroData _heroData;
    private CameraData _cameraData;
    private FieldAnimationData _fieldAnimationData;

    private Sensor m_groundSensorPrefab;
    private HeroView m_heroViewPrefab;
    private GameLevelView m_gameLevelViewPrefab;

    private Camera _camera;

    private readonly IAssetProvider _assetProvider;

    public GamePlayInstaller(IAssetProvider assetProvider) =>
      _assetProvider = assetProvider;

    public async UniTask Preload()
    {
      _heroData = await _assetProvider.Load<HeroData>(HeroDataAddress);
      _cameraData = await _assetProvider.Load<CameraData>(CameraDataAddress);
      _fieldAnimationData = await _assetProvider.Load<FieldAnimationData>(AnimationDataAddress);

      m_heroViewPrefab = (await _assetProvider.Load<GameObject>(HeroViewAddress)).GetComponentInChildren<HeroView>();
      m_gameLevelViewPrefab = (await _assetProvider.Load<GameObject>(GameLevelViewAddress)).GetComponentInChildren<GameLevelView>();
      
      m_groundSensorPrefab = (await _assetProvider.Load<GameObject>(GroundSensorAddress)).GetComponentInChildren<Sensor>();

      _camera = (Object.Instantiate(await _assetProvider.Load<GameObject>(CameraAddress))).GetComponentInChildren<Camera>();
    }

    public void Install(IContainerBuilder builder)
    {
      builder.RegisterInstance(_heroData);
      builder.RegisterInstance(_cameraData);
      builder.RegisterInstance(_fieldAnimationData);
      
      builder.RegisterInstance(m_heroViewPrefab);
      builder.RegisterInstance(m_gameLevelViewPrefab);
        
      builder.RegisterInstance(m_groundSensorPrefab);

      builder.RegisterInstance(_camera);
    }

    public void Clear()
    {
      _assetProvider.Release(HeroDataAddress);
      _assetProvider.Release(CameraDataAddress);
      _assetProvider.Release(AnimationDataAddress);

      _assetProvider.Release(HeroViewAddress);
      _assetProvider.Release(GameLevelViewAddress);
      
      _assetProvider.Release(GroundSensorAddress);

      Object.Destroy(_camera.gameObject);
      _assetProvider.Release(CameraAddress);
    }
  }
}