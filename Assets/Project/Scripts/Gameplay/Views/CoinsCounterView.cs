using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
  public class CoinsCounterView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI m_scoreText;

    public TextMeshProUGUI ScoreText => m_scoreText;
  }
}