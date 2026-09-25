using UnityEngine;

namespace Project.Scripts.Gameplay.Services.BridgeFactory
{
    public interface IGameplayEffectsBridgeFactory
    {
        GameObject CreateDestroyedParticles(Vector3 position);
    }
}
