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
    private const string CanvasAddress = "Canvas";
    private const string HeroDataAddress = "Hero Data";
    private const string SensorsDataAddress = "Sensors Data";
    private const string CameraDataAddress = "Camera Data";
    private const string AnimationDataAddress = "Field Animation Data";
    
    private const string ConnectSensorAddress = "ConnectSensor";
    private const string PersonViewAddress = "PersonView";
    private const string GameLevelViewAddress = "GameLevelView";
    private const string HealthViewAddress = "HealthView";
    
    private const string CameraAddress = "Camera";

    private Canvas _canvasPrefab;
    
    private HeroData _heroData;
    private SensorsData _sensorsData;
    private CameraData _cameraData;
    private FieldAnimationData _fieldAnimationData;

    private Sensor m_connectSensorPrefab;
    private PersonView m_personViewPrefab;
    private GameLevelView m_gameLevelViewPrefab;
    private HealthView m_healthViewPrefab;

    private Camera _camera;

    private readonly IAssetProvider _assetProvider;

    public GamePlayInstaller(IAssetProvider assetProvider) =>
      _assetProvider = assetProvider;

    public async UniTask Preload()
    {
      _canvasPrefab = (await _assetProvider.Load<GameObject>(CanvasAddress)).GetComponentInChildren<Canvas>();
      
      _heroData = await _assetProvider.Load<HeroData>(HeroDataAddress);
      _sensorsData = await _assetProvider.Load<SensorsData>(SensorsDataAddress);
      _cameraData = await _assetProvider.Load<CameraData>(CameraDataAddress);
      _fieldAnimationData = await _assetProvider.Load<FieldAnimationData>(AnimationDataAddress);

      m_personViewPrefab = (await _assetProvider.Load<GameObject>(PersonViewAddress)).GetComponentInChildren<PersonView>();
      m_healthViewPrefab = (await _assetProvider.Load<GameObject>(HealthViewAddress)).GetComponentInChildren<HealthView>();
      m_gameLevelViewPrefab = (await _assetProvider.Load<GameObject>(GameLevelViewAddress)).GetComponentInChildren<GameLevelView>();

      m_connectSensorPrefab = (await _assetProvider.Load<GameObject>(ConnectSensorAddress)).GetComponentInChildren<Sensor>();

      _camera = (Object.Instantiate(await _assetProvider.Load<GameObject>(CameraAddress))).GetComponentInChildren<Camera>();
    }

    public void Install(IContainerBuilder builder)
    {
      builder.RegisterInstance(_canvasPrefab);
      
      builder.RegisterInstance(_heroData);
      builder.RegisterInstance(_sensorsData);
      builder.RegisterInstance(_cameraData);
      builder.RegisterInstance(_fieldAnimationData);
      
      builder.RegisterInstance(m_personViewPrefab);
      builder.RegisterInstance(m_healthViewPrefab);
      builder.RegisterInstance(m_gameLevelViewPrefab);
        
      builder.RegisterInstance(m_connectSensorPrefab);

      builder.RegisterInstance(_camera);
    }

    public void Clear()
    {
      _assetProvider.Release(CanvasAddress);
      
      _assetProvider.Release(HeroDataAddress);
      _assetProvider.Release(SensorsDataAddress);
      _assetProvider.Release(CameraDataAddress);
      _assetProvider.Release(AnimationDataAddress);

      _assetProvider.Release(PersonViewAddress);
      _assetProvider.Release(HealthViewAddress);
      _assetProvider.Release(GameLevelViewAddress);
      
      _assetProvider.Release(ConnectSensorAddress);

      Object.Destroy(_camera.gameObject);
      _assetProvider.Release(CameraAddress);
    }
  }
}