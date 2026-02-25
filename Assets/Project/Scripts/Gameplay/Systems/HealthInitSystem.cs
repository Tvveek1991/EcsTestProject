using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PersonData m_personData;
        
        private EcsWorld m_world;

        private EcsFilter m_personFilter;
        private EcsFilter m_objectsFilter;
        
        private EcsPool<HealthComponent> m_addHealthPool;
        private EcsPool<ObjectComponent> m_objectsPool;

        public HealthInitSystem(PersonData personData)
        {
            m_personData = personData;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_personFilter = m_world.Filter<PersonComponent>().Exc<HealthComponent>().End();
            m_objectsFilter = m_world.Filter<ObjectComponent>().Exc<HealthComponent>().End();
            
            m_addHealthPool = m_world.GetPool<HealthComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            TryAttacheHealthComponent();
        }

        private void TryAttacheHealthComponent()
        {
            foreach (var person in m_personFilter) 
                m_addHealthPool.Add(person).Health = m_personData.FullHealth;
            
            foreach (var objectEntity in m_objectsFilter) 
                m_addHealthPool.Add(objectEntity).Health = m_personData.FullHealth;
        }
    }
}
