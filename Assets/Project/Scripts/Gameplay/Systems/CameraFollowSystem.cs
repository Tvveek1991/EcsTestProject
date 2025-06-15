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
        
        private EcsFilter m_heroFilter;
        
        private EcsPool<TransformComponent> m_transformPool;
        
        private Vector3 m_offset;
        private float m_offsetZ = -10f;
        private float m_smoothSpeed = 0.125f;
        
        public CameraFollowSystem(Camera camera, CameraData cameraData)
        {
            m_camera = camera;
            m_cameraData = cameraData;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_heroFilter = m_world.Filter<HeroComponent>().Inc<TransformComponent>().End();
            m_transformPool = m_world.GetPool<TransformComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var heroIndex in m_heroFilter)
            {
                Transform cameraTransform;
                Vector3 target = m_transformPool.Get(heroIndex).ObjectTransform.position;
                Vector3 desiredPosition = new Vector3(target.x, (cameraTransform = m_camera.transform).position.y, m_offsetZ);
                Vector3 smoothedPosition = Vector3.Lerp(cameraTransform.position, desiredPosition, m_smoothSpeed);
                cameraTransform.position = smoothedPosition;
            }
        }
    }
}
