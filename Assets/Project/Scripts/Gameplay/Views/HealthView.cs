using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Views
{
    public class HealthView : EntityView
    {
        [SerializeField] private Slider m_healthBar;
        [SerializeField] private CanvasGroup m_canvasGroup;

        public Slider HealthBar => m_healthBar;
        public CanvasGroup CanvasGroup => m_canvasGroup;
    }
}
