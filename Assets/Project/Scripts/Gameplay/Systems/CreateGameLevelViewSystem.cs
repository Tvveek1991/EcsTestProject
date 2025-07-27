using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay
{
    public sealed class CreateGameLevelViewSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private const string GameLevelParentName = "Level";
        
        private readonly GameLevelView m_gameLevelViewPrefab;
        
        private EcsWorld _world;
        
        private EcsFilter m_gameLevelViewRefFilter;
        
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;
        
        private GameObject m_parentObject;
        
        public CreateGameLevelViewSystem(GameLevelView gameLevelViewPrefab)
        {
            m_gameLevelViewPrefab = gameLevelViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            m_gameLevelViewRefFilter = _world.Filter<GameLevelViewRefComponent>().End();
            m_gameLevelViewRefsPool = _world.GetPool<GameLevelViewRefComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            if(m_gameLevelViewRefFilter.GetEntitiesCount() > 0)
                return;
            
            CreateGameLevelView();
        }

        private void CreateGameLevelView()
        {
            m_parentObject = new GameObject(GameLevelParentName);
            
            var gameLevelEntityIndex = _world.NewEntity();

            var levelView = Object.Instantiate(m_gameLevelViewPrefab, m_parentObject.transform).GetComponent<GameLevelView>();
                
            AttachViewToGameLevelViewReference();

            void AttachViewToGameLevelViewReference()
            {
                ref GameLevelViewRefComponent cellViewRef = ref m_gameLevelViewRefsPool.Add(gameLevelEntityIndex);
                cellViewRef.GameLevelView = levelView;
            }
        }
        
        public void Destroy(IEcsSystems systems) => 
            Object.Destroy(m_parentObject);
    }
}