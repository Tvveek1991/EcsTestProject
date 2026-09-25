using UnityEngine;

namespace Project.Scripts.Gameplay.Services.BridgeFactory
{
    public sealed class GameplayEffectsBridgeFactory : IGameplayEffectsBridgeFactory
    {
        private readonly GameObject m_destroyedParticlesPrefab;

        public GameplayEffectsBridgeFactory(GameObject destroyedParticlesPrefab)
        {
            m_destroyedParticlesPrefab = destroyedParticlesPrefab;
        }

        public GameObject CreateDestroyedParticles(Vector3 position) =>
            Object.Instantiate(m_destroyedParticlesPrefab, position, Quaternion.identity);
    }
}
