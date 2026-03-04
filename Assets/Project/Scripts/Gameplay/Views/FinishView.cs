using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Views
{
    public class FinishView : MonoBehaviour
    {
        [SerializeField] private Button m_restartButton;
        [SerializeField] private TextMeshProUGUI m_title;
        [SerializeField] private TextMeshProUGUI m_buttonText;
        [SerializeField] private CanvasGroup m_canvasGroup;

        public TextMeshProUGUI Title => m_title;
        public TextMeshProUGUI ButtonText => m_buttonText;
        public Button RestartButton => m_restartButton;
        public CanvasGroup CanvasGroup => m_canvasGroup;
    }
}
