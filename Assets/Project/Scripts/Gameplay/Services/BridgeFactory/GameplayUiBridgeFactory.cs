using Project.Scripts.Gameplay.Views;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.BridgeFactory
{
    public sealed class GameplayUiBridgeFactory : IGameplayUiBridgeFactory
    {
        private readonly FinishView m_finishViewPrefab;
        private readonly TextMeshProUGUI m_tutorialViewPrefab;

        public GameplayUiBridgeFactory(FinishView finishViewPrefab, TextMeshProUGUI tutorialViewPrefab)
        {
            m_finishViewPrefab = finishViewPrefab;
            m_tutorialViewPrefab = tutorialViewPrefab;
        }

        public FinishView CreateFinish(Transform parent) =>
            Object.Instantiate(m_finishViewPrefab, parent);

        public GameObject CreateTutorial(Transform parent) =>
            Object.Instantiate(m_tutorialViewPrefab.gameObject, parent);
    }
}
