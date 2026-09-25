using Project.Scripts.Gameplay.Sensors;
using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class CoinView : EntityView
    {
        [SerializeField] private Sensor m_sensor;

        public Sensor Sensor => m_sensor;
    }
}
