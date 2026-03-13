using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CameraService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CameraFollowSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly ICameraService m_cameraService;
        
        private EcsWorld m_world;
        
        private EcsFilter m_playerFilter;
        
        private EcsPool<TransformKeeper> m_transformPool;
        
        private Vector3 m_offset;
        
        public CameraFollowSystem(ICameraService cameraService)
        {
            m_cameraService = cameraService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_playerFilter = m_world.Filter<Player>().Inc<TransformKeeper>().End(1);
            
            m_transformPool = m_world.GetPool<TransformKeeper>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var index in m_playerFilter)
            {
                Transform cameraTransform;
                Vector3 target = m_transformPool.Get(index).ObjectTransform.position;
                
                Vector3 desiredPosition = new Vector3(target.x, (cameraTransform = m_cameraService.Camera.transform).position.y, m_cameraService.CameraData.OffsetZ);
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, m_cameraService.CameraData.MinPositionX, m_cameraService.CameraData.MaxPositionX);
                
                Vector3 smoothedPosition = Vector3.Lerp(cameraTransform.position, desiredPosition, m_cameraService.CameraData.SmoothSpeed);
                cameraTransform.position = smoothedPosition;
            }
        }
    }
}
