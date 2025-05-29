using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay
{
    public sealed class CameraResizeInitSystem : IEcsInitSystem
    {
        private readonly Camera m_camera;
        private readonly CameraData m_cameraData;
        
        private EcsWorld _world;

        private EcsFilter m_gameLevelFilter;
        
        private EcsPool<GameLevelViewRefComponent> m_gameLevelViewRefPool;
        
        public CameraResizeInitSystem(Camera camera, CameraData cameraData)
        {
            m_camera = camera;
            m_cameraData = cameraData;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            m_gameLevelFilter = _world.Filter<GameLevelViewRefComponent>().End();
            
            m_gameLevelViewRefPool = _world.GetPool<GameLevelViewRefComponent>();

            SetCameraCenter();
        }

        private void SetCameraCenter()
        {
            
            foreach (var gameLevel in m_gameLevelFilter)
            {
                Vector2 newCameraCenter = m_gameLevelViewRefPool.Get(gameLevel).GameLevelView.GetHeroSpawnPoint();
                newCameraCenter += m_cameraData.FieldViewCenterOffset;
      
                m_camera.transform.position = new Vector3(newCameraCenter.x, newCameraCenter.y, m_camera.transform.position.z);
            }
        }
    }
}