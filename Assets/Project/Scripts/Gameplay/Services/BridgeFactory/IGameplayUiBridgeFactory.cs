using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.BridgeFactory
{
    public interface IGameplayUiBridgeFactory
    {
        FinishView CreateFinish(Transform parent);
        GameObject CreateTutorial(Transform parent);
    }
}
