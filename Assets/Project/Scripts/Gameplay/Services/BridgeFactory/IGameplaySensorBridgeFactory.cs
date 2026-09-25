using Project.Scripts.Gameplay.Sensors;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.BridgeFactory
{
    public interface IGameplaySensorBridgeFactory
    {
        Sensor CreateSensor(Transform parent, Vector2 localPosition);
    }
}
