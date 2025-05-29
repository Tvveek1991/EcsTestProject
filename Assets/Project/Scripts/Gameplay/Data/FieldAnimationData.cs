using DG.Tweening;
using UnityEngine;

namespace Gameplay.Data
{
  [CreateAssetMenu(fileName = "Field Animation Data", menuName = "Animation/Create Field Animation Data", order = 0)]
  public class FieldAnimationData : ScriptableObject
  {
    public float Duration;
    public Ease Ease;
  }
}