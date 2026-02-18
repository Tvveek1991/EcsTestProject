using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Gameplay.Data
{
    [CreateAssetMenu(menuName = "Hero/Sensors Data", fileName = "Sensors Data")]
    public class SensorsData : ScriptableObject
    {
        public List<Vector2> GroundSensorPosition;
        public List<Vector2> WallSensorsPosition;
    }
}
