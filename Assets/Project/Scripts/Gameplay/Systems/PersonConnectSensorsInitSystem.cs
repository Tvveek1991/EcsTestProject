using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Services.BridgeFactory;
using Project.Scripts.Gameplay.Sensors;

namespace Project.Scripts.Gameplay.Systems
{
    public class PersonConnectSensorsInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_wallCheckFilter;
        private EcsFilter m_groundCheckFilter;

        private EcsPool<WallCheck> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<TransformKeeper> m_transformPool;

        private readonly SensorsData m_sensorsData;
        private readonly IGameplaySensorBridgeFactory m_gameplaySensorBridgeFactory;

        public PersonConnectSensorsInitSystem(SensorsData sensorsData, IGameplaySensorBridgeFactory gameplaySensorBridgeFactory)
        {
            m_sensorsData = sensorsData;
            m_gameplaySensorBridgeFactory = gameplaySensorBridgeFactory;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_wallCheckFilter = m_world.Filter<WallCheck>().Inc<TransformKeeper>().End();
            m_groundCheckFilter = m_world.Filter<GroundCheckComponent>().Inc<TransformKeeper>().End();

            m_transformPool = m_world.GetPool<TransformKeeper>();
            m_wallCheckPool = m_world.GetPool<WallCheck>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();

            CreateGroundCheckSensor();
            CreateWallCheckSensors();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var item in m_groundCheckFilter)
            {
                m_groundCheckPool.Get(item).GroundSensors.ForEach(sensor => sensor.SubtractTimer());
            }
        }

        private void CreateGroundCheckSensor()
        {
            foreach (var item in m_groundCheckFilter)
            {
                m_groundCheckPool.Get(item).GroundSensors = new List<Sensor>();
                foreach (var sensorPosition in m_sensorsData.GroundSensorPosition)
                {
                    var groundSensor = m_gameplaySensorBridgeFactory.CreateSensor(m_transformPool.Get(item).ObjectTransform, sensorPosition);
                    m_groundCheckPool.Get(item).GroundSensors.Add(groundSensor);
                }
            }
        }

        private void CreateWallCheckSensors()
        {
            foreach (var item in m_wallCheckFilter)
            {
                m_wallCheckPool.Get(item).WallSensors = new List<Sensor>();
                foreach (var sensorPosition in m_sensorsData.WallSensorsPosition)
                {
                    var wallSensor = m_gameplaySensorBridgeFactory.CreateSensor(m_transformPool.Get(item).ObjectTransform, sensorPosition);
                    m_wallCheckPool.Get(item).WallSensors.Add(wallSensor);
                }
            }
        }
    }
}
