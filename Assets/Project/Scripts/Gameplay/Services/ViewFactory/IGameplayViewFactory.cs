using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.ViewFactory
{
    public interface IGameplayViewFactory
    {
        PersonView CreatePlayer(Transform parent);
        ObjectView CreateBox(Transform parent);
        CoinView CreateCoin(Transform parent);
        HealthView CreateHealth(Transform parent);
    }
}
