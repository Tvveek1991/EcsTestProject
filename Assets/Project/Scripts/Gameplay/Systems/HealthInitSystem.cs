using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_personFilter;
        
        private EcsPool<HealthComponent> m_addHealthPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_personFilter = m_world.Filter<PersonComponent>().Exc<HealthComponent>().End();
            
            m_addHealthPool = m_world.GetPool<HealthComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            TryAttacheHealthComponent();
        }

        private void TryAttacheHealthComponent()
        {
            foreach (var person in m_personFilter) 
                m_addHealthPool.Add(person);
        }
    }
}
