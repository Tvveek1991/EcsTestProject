using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.ViewFactory
{
    public sealed class GameplayViewFactory : IGameplayViewFactory
    {
        private readonly PersonView m_personViewPrefab;
        private readonly ObjectView m_objectViewPrefab;
        private readonly CoinView m_coinViewPrefab;
        private readonly HealthView m_healthViewPrefab;

        public GameplayViewFactory(PersonView personViewPrefab, ObjectView objectViewPrefab, CoinView coinViewPrefab, HealthView healthViewPrefab)
        {
            m_personViewPrefab = personViewPrefab;
            m_objectViewPrefab = objectViewPrefab;
            m_coinViewPrefab = coinViewPrefab;
            m_healthViewPrefab = healthViewPrefab;
        }

        public PersonView CreatePlayer(Transform parent) =>
            Object.Instantiate(m_personViewPrefab, parent);

        public ObjectView CreateBox(Transform parent) =>
            Object.Instantiate(m_objectViewPrefab, parent);

        public CoinView CreateCoin(Transform parent) =>
            Object.Instantiate(m_coinViewPrefab, parent);

        public HealthView CreateHealth(Transform parent) =>
            Object.Instantiate(m_healthViewPrefab, parent);
    }
}
