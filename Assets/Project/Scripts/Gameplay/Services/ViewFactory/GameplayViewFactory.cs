using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.ViewFactory
{
    public sealed class GameplayViewFactory : IGameplayViewFactory
    {
        private readonly Canvas m_canvasPrefab;
        private readonly PersonView m_personViewPrefab;
        private readonly ObjectView m_objectViewPrefab;
        private readonly CoinView m_coinViewPrefab;
        private readonly HealthView m_healthViewPrefab;
        private readonly GameLevelView m_gameLevelViewPrefab;
        private readonly CoinsCounterView m_coinsCounterViewPrefab;

        public GameplayViewFactory(Canvas canvasPrefab, PersonView personViewPrefab, ObjectView objectViewPrefab, CoinView coinViewPrefab,
            HealthView healthViewPrefab, GameLevelView gameLevelViewPrefab, CoinsCounterView coinsCounterViewPrefab)
        {
            m_canvasPrefab = canvasPrefab;
            m_personViewPrefab = personViewPrefab;
            m_objectViewPrefab = objectViewPrefab;
            m_coinViewPrefab = coinViewPrefab;
            m_healthViewPrefab = healthViewPrefab;
            m_gameLevelViewPrefab = gameLevelViewPrefab;
            m_coinsCounterViewPrefab = coinsCounterViewPrefab;
        }

        public Canvas CreateCanvas() =>
            Object.Instantiate(m_canvasPrefab);

        public PersonView CreatePlayer(Transform parent) =>
            Object.Instantiate(m_personViewPrefab, parent);

        public ObjectView CreateBox(Transform parent) =>
            Object.Instantiate(m_objectViewPrefab, parent);

        public CoinView CreateCoin(Transform parent) =>
            Object.Instantiate(m_coinViewPrefab, parent);

        public HealthView CreateHealth(Transform parent) =>
            Object.Instantiate(m_healthViewPrefab, parent);

        public GameLevelView CreateGameLevel(Transform parent) =>
            Object.Instantiate(m_gameLevelViewPrefab, parent);

        public CoinsCounterView CreateCoinsCounter(Transform parent) =>
            Object.Instantiate(m_coinsCounterViewPrefab, parent);
    }
}
