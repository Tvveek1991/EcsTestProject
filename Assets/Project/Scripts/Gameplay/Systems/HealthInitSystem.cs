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
        
        private EcsPool<Health> m_addHealthPool;
        private EcsPool<PlayableObject> m_objectsPool;

        public HealthInitSystem(PersonData personData)
        {
            m_personData = personData;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_personFilter = m_world.Filter<Person>().Exc<Health>().End();
            m_objectsFilter = m_world.Filter<PlayableObject>().Exc<Health>().End();
            
            m_addHealthPool = m_world.GetPool<Health>();
        }

        public void Run(IEcsSystems systems)
        {
            AttacheHealthComponent();
        }

        private void AttacheHealthComponent()
        {
            foreach (var person in m_personFilter) 
                m_addHealthPool.Add(person).Count = m_personData.FullHealth;
            
            foreach (var objectEntity in m_objectsFilter) 
                m_addHealthPool.Add(objectEntity).Count = m_personData.FullHealth;
        }
    }
}
