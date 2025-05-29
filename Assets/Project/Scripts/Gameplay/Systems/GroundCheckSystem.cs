using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Sensors;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class GroundCheckSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_groundCheckFilter;

        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<TransformComponent> m_transformPool;

        private readonly HeroData m_heroData;
        private readonly Sensor m_groundSensorPrefab;

        public GroundCheckSystem(HeroData heroData, Sensor groundSensorPrefab)
        {
            m_heroData = heroData;
            m_groundSensorPrefab = groundSensorPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_groundCheckFilter = m_world.Filter<GroundCheckComponent>().Inc<TransformComponent>().End();
            
            m_transformPool = m_world.GetPool<TransformComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();

            CreateGroundCheckSensor();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var item in m_groundCheckFilter)
            {
                m_groundCheckPool.Get(item).GroundSensor.SubtractTimer();
            }
        }

        private void CreateGroundCheckSensor()
        {
            foreach (var item in m_groundCheckFilter)
            {
                var groundSensor = Object.Instantiate(m_groundSensorPrefab, m_transformPool.Get(item).ObjectTransform).GetComponent<Sensor>();
                
                var transform = groundSensor.transform;
                var newPosition = transform.position;
                newPosition.y += m_heroData.GroundSensorOffset;
                transform.position = newPosition;
                
                m_groundCheckPool.Get(item).GroundSensor = groundSensor;
            }
        }
    }
}
