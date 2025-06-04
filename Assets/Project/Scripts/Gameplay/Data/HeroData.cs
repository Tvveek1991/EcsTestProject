using UnityEngine;

namespace Project.Scripts.Gameplay.Data
{
    [CreateAssetMenu(menuName = "Hero/Create Hero Data", fileName = "Hero Data")]
    public class HeroData : ScriptableObject
    {
        public float Speed = 4.0f;
        public float JumpForce = 7.5f;
        public float RollForce = 6.0f;
    }
}