using UnityEngine;

namespace Project.Scripts.Gameplay.Data
{
    [CreateAssetMenu(fileName = "Camera Data", menuName = "Camera/Create CameraData", order = 0)]
    public class CameraData : ScriptableObject
    {
        public Vector2 FieldViewCenterOffset;
    }
}
