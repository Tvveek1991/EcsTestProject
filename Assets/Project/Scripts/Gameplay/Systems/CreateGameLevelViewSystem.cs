using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay
{
    public sealed class CreateGameLevelViewSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const string GameLevelParentName = "Level";
        
        private readonly GameLevelView m_gameLevelViewPrefab;
        
        private EcsWorld m_world;
        
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefsPool;
        
        private GameObject m_parentObject;
        
        public CreateGameLevelViewSystem(GameLevelView gameLevelViewPrefab)
        {
            m_gameLevelViewPrefab = gameLevelViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_gameLevelViewRefsPool = m_world.GetPool<GameLevelViewRefComponent>();

            CreateGameLevelView();
        }

        private void CreateGameLevelView()
        {
            m_parentObject = new GameObject(GameLevelParentName);
            
            var gameLevelEntityIndex = m_world.NewEntity();

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