using UnityEngine;

namespace Project.Scripts.Gameplay.Data
{
    [CreateAssetMenu(fileName = "Camera Data", menuName = "Camera/Create CameraData", order = 0)]
    public class CameraData : ScriptableObject
    {
        public Vector2 FieldViewCenterOffset;
        public float OffsetZ = -10f;
        public float SmoothSpeed = 0.125f;
        public int MinPositionX = -4;
        public int MaxPositionX = 110;
    }
}
