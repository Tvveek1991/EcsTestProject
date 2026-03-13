using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Services.GameLevelService;
using UnityEngine;

namespace Project.Scripts.Gameplay
{
    public sealed class CameraInitSystem : IEcsInitSystem
    {
        private readonly Camera m_camera;
        private readonly CameraData m_cameraData;
        private readonly IGameLevelService m_gameLevelService;

        private EcsWorld m_world;
        
        private EcsFilter m_cameraFilter;

        private EcsPool<CameraKeeper> m_cameraPool;
        private EcsPool<TransformKeeper> m_transformPool;

        public CameraInitSystem(Camera camera, CameraData cameraData, IGameLevelService gameLevelService)
        {
            m_camera = camera;
            m_cameraData = cameraData;
            m_gameLevelService = gameLevelService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_cameraFilter = m_world.Filter<CameraKeeper>().Inc<TransformKeeper>().End(1);
            
            m_cameraPool = m_world.GetPool<CameraKeeper>();
            m_transformPool = m_world.GetPool<TransformKeeper>();

            AttachComponents();

            SetCameraCenter();
        }

        private void AttachComponents()
        {
            var cameraEntity = m_world.NewEntity();
            m_cameraPool.Add(cameraEntity);

            var cameraTransform = m_camera.transform;
            ref TransformKeeper transformKeeper = ref m_transformPool.Add(cameraEntity);
            transformKeeper.ObjectTransform = cameraTransform;
        }

        private void SetCameraCenter()
        {
            foreach (var entity in m_cameraFilter)
            {
                Vector2 newCameraCenter = m_gameLevelService.View.GetHeroSpawnPoint();
                newCameraCenter += m_cameraData.FieldViewCenterOffset;

                var cameraTransform = m_transformPool.Get(entity).ObjectTransform;
                cameraTransform.position = new Vector3(newCameraCenter.x, newCameraCenter.y, cameraTransform.position.z);
            }
        }
    }
}