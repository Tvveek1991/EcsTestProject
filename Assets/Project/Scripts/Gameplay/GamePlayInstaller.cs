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
    private const string PersonDataAddress = "Person Data";
    private const string SensorsDataAddress = "Sensors Data";
    private const string CameraDataAddress = "Camera Data";
    private const string AnimationDataAddress = "Field Animation Data";
    
    private const string ConnectSensorAddress = "ConnectSensor";
    private const string PersonViewAddress = "PersonView";
    private const string GameLevelViewAddress = "GameLevelView";
    private const string HealthViewAddress = "HealthView";
    private const string FinishViewAddress = "FinishView";
    private const string CoinViewAddress = "CoinView";
    private const string BoxViewAddress = "BoxView";
    private const string CoinsCounterViewAddress = "CoinsCounterView";
    
    private const string CameraAddress = "Camera";

    private Canvas _canvasPrefab;
    
    private PersonData m_personData;
    private SensorsData _sensorsData;
    private CameraData _cameraData;
    private FieldAnimationData _fieldAnimationData;

    private Sensor m_connectSensorPrefab;
    private PersonView m_personViewPrefab;
    private GameLevelView m_gameLevelViewPrefab;
    private HealthView m_healthViewPrefab;
    private FinishView m_finishViewPrefab;
    private CoinView m_coinViewPrefab;
    private ObjectView m_objectViewPrefab;
    private CoinsCounterView m_coinsCounterViewPrefab;

    private Camera _camera;

    private readonly IAssetProvider _assetProvider;

    public GamePlayInstaller(IAssetProvider assetProvider) =>
      _assetProvider = assetProvider;

    public async UniTask Preload()
    {
      _canvasPrefab = (await _assetProvider.Load<GameObject>(CanvasAddress)).GetComponentInChildren<Canvas>();
      
      m_personData = await _assetProvider.Load<PersonData>(PersonDataAddress);
      _sensorsData = await _assetProvider.Load<SensorsData>(SensorsDataAddress);
      _cameraData = await _assetProvider.Load<CameraData>(CameraDataAddress);
      _fieldAnimationData = await _assetProvider.Load<FieldAnimationData>(AnimationDataAddress);

      m_personViewPrefab = (await _assetProvider.Load<GameObject>(PersonViewAddress)).GetComponentInChildren<PersonView>();
      m_finishViewPrefab = (await _assetProvider.Load<GameObject>(FinishViewAddress)).GetComponentInChildren<FinishView>();
      m_healthViewPrefab = (await _assetProvider.Load<GameObject>(HealthViewAddress)).GetComponentInChildren<HealthView>();
      m_gameLevelViewPrefab = (await _assetProvider.Load<GameObject>(GameLevelViewAddress)).GetComponentInChildren<GameLevelView>();
      m_coinViewPrefab = (await _assetProvider.Load<GameObject>(CoinViewAddress)).GetComponentInChildren<CoinView>();
      m_objectViewPrefab = (await _assetProvider.Load<GameObject>(BoxViewAddress)).GetComponentInChildren<ObjectView>();
      m_coinsCounterViewPrefab = (await _assetProvider.Load<GameObject>(CoinsCounterViewAddress)).GetComponentInChildren<CoinsCounterView>();

      m_connectSensorPrefab = (await _assetProvider.Load<GameObject>(ConnectSensorAddress)).GetComponentInChildren<Sensor>();

      _camera = (Object.Instantiate(await _assetProvider.Load<GameObject>(CameraAddress))).GetComponentInChildren<Camera>();
    }

    public void Install(IContainerBuilder builder)
    {
      builder.RegisterInstance(_canvasPrefab);
      
      builder.RegisterInstance(m_personData);
      builder.RegisterInstance(_sensorsData);
      builder.RegisterInstance(_cameraData);
      builder.RegisterInstance(_fieldAnimationData);
      
      builder.RegisterInstance(m_personViewPrefab);
      builder.RegisterInstance(m_finishViewPrefab);
      builder.RegisterInstance(m_healthViewPrefab);
      builder.RegisterInstance(m_gameLevelViewPrefab);
      builder.RegisterInstance(m_coinViewPrefab);
      builder.RegisterInstance(m_objectViewPrefab);
      builder.RegisterInstance(m_coinsCounterViewPrefab);
        
      builder.RegisterInstance(m_connectSensorPrefab);

      builder.RegisterInstance(_camera);
    }

    public void Clear()
    {
      _assetProvider.Release(CanvasAddress);
      
      _assetProvider.Release(PersonDataAddress);
      _assetProvider.Release(SensorsDataAddress);
      _assetProvider.Release(CameraDataAddress);
      _assetProvider.Release(AnimationDataAddress);

      _assetProvider.Release(PersonViewAddress);
      _assetProvider.Release(FinishViewAddress);
      _assetProvider.Release(HealthViewAddress);
      _assetProvider.Release(GameLevelViewAddress);
      _assetProvider.Release(CoinViewAddress);
      _assetProvider.Release(BoxViewAddress);
      _assetProvider.Release(CoinsCounterViewAddress);
      
      _assetProvider.Release(ConnectSensorAddress);

      Object.Destroy(_camera.gameObject);
      _assetProvider.Release(CameraAddress);
    }
  }
}