using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CanvasService;
using Project.Scripts.Gameplay.Services.CoinsCounterService;
using Project.Scripts.Gameplay.Services.ViewFactory;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsCounterViewInitSystem : IEcsInitSystem
    {
        private readonly IGameplayViewFactory m_gameplayViewFactory;
        private readonly ICoinsCounterService m_coinsCounterService;
        private readonly ICanvasService m_canvasService;

        private EcsWorld m_world;

        private EcsPool<CoinsCounterViewKeeper> m_coinsCounterViewPool;

        public CoinsCounterViewInitSystem(IGameplayViewFactory gameplayViewFactory, ICoinsCounterService coinsCounterService, ICanvasService canvasService)
        {
            m_canvasService = canvasService;
            m_coinsCounterService = coinsCounterService;
            m_gameplayViewFactory = gameplayViewFactory;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_coinsCounterViewPool = m_world.GetPool<CoinsCounterViewKeeper>();
            
            CreateCoinsCounterView();
        }

        private void CreateCoinsCounterView()
        {
            var newEntity = m_world.NewEntity();
            m_coinsCounterViewPool.Add(newEntity);
            
            var spawnPoint = m_canvasService.Canvas.transform;
            var view = m_gameplayViewFactory.CreateCoinsCounter(spawnPoint);
            view.ScoreText.text = "0";

            m_coinsCounterService.Construct(newEntity, view);
        }
    }
}
