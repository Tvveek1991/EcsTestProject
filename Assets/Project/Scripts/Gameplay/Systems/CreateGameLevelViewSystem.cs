using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.GameLevelService;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay
{
    public sealed class CreateGameLevelViewSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string GameLevelParentName = "Level";
        
        private readonly GameLevelView m_gameLevelViewPrefab;
        private readonly IGameLevelService m_gameLevelService;

        private EcsWorld m_world;
        
        private EcsPool<GameLevel> m_gameLevelPool;
        
        private GameObject m_parentObject;
        
        public CreateGameLevelViewSystem(GameLevelView gameLevelViewPrefab, IGameLevelService gameLevelService)
        {
            m_gameLevelService = gameLevelService;
            m_gameLevelViewPrefab = gameLevelViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_gameLevelPool = m_world.GetPool<GameLevel>();

            CreateGameLevelView();
        }

        private void CreateGameLevelView()
        {
            m_parentObject = new GameObject(GameLevelParentName);
            
            var entity = m_world.NewEntity();
            m_gameLevelPool.Add(entity);

            var levelView = Object.Instantiate(m_gameLevelViewPrefab, m_parentObject.transform);
            m_gameLevelService.Construct(entity, levelView);
        }
        
        public void Destroy(IEcsSystems systems) => 
            Object.Destroy(m_parentObject);
    }
}