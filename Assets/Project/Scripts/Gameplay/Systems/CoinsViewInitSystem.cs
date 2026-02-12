using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Systems
{
    public class CoinsViewInitSystem : IEcsInitSystem
    {
        private readonly CoinsCounterView m_coinsCounterViewPrefab;
        
        private EcsWorld m_world;
        
        public CoinsViewInitSystem(CoinsCounterView coinsCounterViewPrefab) => 
            m_coinsCounterViewPrefab = coinsCounterViewPrefab;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
        }
    }
}