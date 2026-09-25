using Project.Scripts.Gameplay.Sensors;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.BridgeFactory
{
    public sealed class GameplaySensorBridgeFactory : IGameplaySensorBridgeFactory
    {
        private readonly Sensor m_sensorPrefab;

        public GameplaySensorBridgeFactory(Sensor sensorPrefab)
        {
            m_sensorPrefab = sensorPrefab;
        }

        public Sensor CreateSensor(Transform parent, Vector2 localPosition)
        {
            var sensor = Object.Instantiate(m_sensorPrefab, parent);
            sensor.transform.localPosition = localPosition;
            return sensor;
        }
    }
}
