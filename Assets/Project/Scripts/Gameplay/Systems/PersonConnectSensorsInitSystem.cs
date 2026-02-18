using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using Project.Scripts.Gameplay.Sensors;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class PersonConnectSensorsInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_wallCheckFilter;
        private EcsFilter m_groundCheckFilter;

        private EcsPool<WallCheckComponent> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<TransformComponent> m_transformPool;

        private readonly SensorsData m_sensorsData;
        private readonly Sensor m_connectSensorPrefab;

        public PersonConnectSensorsInitSystem(SensorsData sensorsData, Sensor connectSensorPrefab)
        {
            m_sensorsData = sensorsData;
            m_connectSensorPrefab = connectSensorPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_wallCheckFilter = m_world.Filter<WallCheckComponent>().Inc<TransformComponent>().End();
            m_groundCheckFilter = m_world.Filter<GroundCheckComponent>().Inc<TransformComponent>().End();

            m_transformPool = m_world.GetPool<TransformComponent>();
            m_wallCheckPool = m_world.GetPool<WallCheckComponent>();
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
                    var groundSensor = Object.Instantiate(m_connectSensorPrefab, m_transformPool.Get(item).ObjectTransform).GetComponent<Sensor>();
                    groundSensor.transform.localPosition = sensorPosition;
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
                    var wallSensor = Object.Instantiate(m_connectSensorPrefab, m_transformPool.Get(item).ObjectTransform).GetComponent<Sensor>();
                    wallSensor.transform.localPosition = sensorPosition;
                    m_wallCheckPool.Get(item).WallSensors.Add(wallSensor);
                }
            }
        }
    }
}
