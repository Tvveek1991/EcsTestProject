using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class HeroView : MonoBehaviour
    {
        public void SetPosition(Vector3 newPosition) => 
            transform.position = newPosition;
    }
}