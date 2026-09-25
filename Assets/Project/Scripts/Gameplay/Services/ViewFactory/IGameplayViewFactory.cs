using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.ViewFactory
{
    public interface IGameplayViewFactory
    {
        Canvas CreateCanvas();
        PersonView CreatePlayer(Transform parent);
        ObjectView CreateBox(Transform parent);
        CoinView CreateCoin(Transform parent);
        HealthView CreateHealth(Transform parent);
        GameLevelView CreateGameLevel(Transform parent);
        CoinsCounterView CreateCoinsCounter(Transform parent);
    }
}
