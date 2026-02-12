using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
  public class CoinsCounterView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI m_scoreText;

    private void Awake() => 
      SetCount(0);

    public void SetCount(int score) => 
      m_scoreText.text = $": {score}";
  }
}