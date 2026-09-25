using System.Threading;
using AssetProvider.Scripts;
using Cysharp.Threading.Tasks;
using Gameplay.Data;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Sensors;
using Project.Scripts.Gameplay.Views;
using TMPro;
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
    
    private const string TutorialViewAddress = "TutorialView";
    
    private const string DestroyedParticlesAddress = "DestroyedParticles";
    
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
    
    private GameObject m_destroyedParticlesPrefab;
    private TextMeshProUGUI m_tutorialViewPrefab;

    private Camera _camera;

    private readonly IAssetProvider _assetProvider;

    public GamePlayInstaller(IAssetProvider assetProvider) =>
      _assetProvider = assetProvider;

    public async UniTask Preload(CancellationToken cancellationToken)
    {
      _canvasPrefab = (await Load<GameObject>(CanvasAddress, cancellationToken)).GetComponentInChildren<Canvas>();
      
      m_personData = await Load<PersonData>(PersonDataAddress, cancellationToken);
      _sensorsData = await Load<SensorsData>(SensorsDataAddress, cancellationToken);
      _cameraData = await Load<CameraData>(CameraDataAddress, cancellationToken);
      _fieldAnimationData = await Load<FieldAnimationData>(AnimationDataAddress, cancellationToken);

      m_tutorialViewPrefab = (await Load<GameObject>(TutorialViewAddress, cancellationToken)).GetComponentInChildren<TextMeshProUGUI>();
      
      m_personViewPrefab = (await Load<GameObject>(PersonViewAddress, cancellationToken)).GetComponentInChildren<PersonView>();
      m_finishViewPrefab = (await Load<GameObject>(FinishViewAddress, cancellationToken)).GetComponentInChildren<FinishView>();
      m_healthViewPrefab = (await Load<GameObject>(HealthViewAddress, cancellationToken)).GetComponentInChildren<HealthView>();
      m_gameLevelViewPrefab = (await Load<GameObject>(GameLevelViewAddress, cancellationToken)).GetComponentInChildren<GameLevelView>();
      m_coinViewPrefab = (await Load<GameObject>(CoinViewAddress, cancellationToken)).GetComponentInChildren<CoinView>();
      m_objectViewPrefab = (await Load<GameObject>(BoxViewAddress, cancellationToken)).GetComponentInChildren<ObjectView>();
      m_coinsCounterViewPrefab = (await Load<GameObject>(CoinsCounterViewAddress, cancellationToken)).GetComponentInChildren<CoinsCounterView>();

      m_connectSensorPrefab = (await Load<GameObject>(ConnectSensorAddress, cancellationToken)).GetComponentInChildren<Sensor>();

      m_destroyedParticlesPrefab = await Load<GameObject>(DestroyedParticlesAddress, cancellationToken);

      _camera = Camera.main == null ? (Object.Instantiate(await Load<GameObject>(CameraAddress, cancellationToken))).GetComponentInChildren<Camera>() : Camera.main;
    }

    public void Install(IContainerBuilder builder)
    {
      builder.RegisterInstance(_canvasPrefab);
      
      builder.RegisterInstance(m_personData);
      builder.RegisterInstance(_sensorsData);
      builder.RegisterInstance(_cameraData);
      builder.RegisterInstance(_fieldAnimationData);

      builder.RegisterInstance(m_destroyedParticlesPrefab);
      builder.RegisterInstance(m_tutorialViewPrefab);
      
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
      
      _assetProvider.Release(DestroyedParticlesAddress);
      _assetProvider.Release(TutorialViewAddress);

      _assetProvider.Release(PersonViewAddress);
      _assetProvider.Release(FinishViewAddress);
      _assetProvider.Release(HealthViewAddress);
      _assetProvider.Release(GameLevelViewAddress);
      _assetProvider.Release(CoinViewAddress);
      _assetProvider.Release(BoxViewAddress);
      _assetProvider.Release(CoinsCounterViewAddress);
      
      _assetProvider.Release(ConnectSensorAddress);

      // Object.Destroy(_camera.gameObject);
      _assetProvider.Release(CameraAddress);
    }

    private async UniTask<T> Load<T>(string address, CancellationToken cancellationToken) where T : class
    {
      cancellationToken.ThrowIfCancellationRequested();

      T asset = await _assetProvider.Load<T>(address);

      cancellationToken.ThrowIfCancellationRequested();
      return asset;
    }
  }
}
