using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CameraFollowSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly Camera m_camera;
        private readonly CameraData m_cameraData;
        
        private EcsWorld m_world;
        
        private EcsFilter m_playerFilter;
        
        private EcsPool<TransformComponent> m_transformPool;
        
        private Vector3 m_offset;
        
        public CameraFollowSystem(Camera camera, CameraData cameraData)
        {
            m_camera = camera;
            m_cameraData = cameraData;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_playerFilter = m_world.Filter<PlayerComponent>().Inc<TransformComponent>().End();
            m_transformPool = m_world.GetPool<TransformComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var index in m_playerFilter)
            {
                Transform cameraTransform;
                Vector3 target = m_transformPool.Get(index).ObjectTransform.position;
                Vector3 desiredPosition = new Vector3(target.x, (cameraTransform = m_camera.transform).position.y, m_cameraData.OffsetZ);
                Vector3 smoothedPosition = Vector3.Lerp(cameraTransform.position, desiredPosition, m_cameraData.SmoothSpeed);
                cameraTransform.position = smoothedPosition;
            }
        }
    }
}
