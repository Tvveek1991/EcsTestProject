using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CreateHealthViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        private EcsFilter m_healthFilter;
        
        private EcsPool<PersonViewRefComponent> m_heroViewPool;
        private EcsPool<HealthViewRefComponent> m_healthViewPool;
        
        private readonly HealthView m_healthViewPrefab;

        public CreateHealthViewSystem(HealthView healthViewPrefab)
        {
            m_healthViewPrefab = healthViewPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_healthFilter = m_world.Filter<HealthComponent>().Inc<PersonViewRefComponent>().Exc<HealthViewRefComponent>().End();
            
            m_heroViewPool = m_world.GetPool<PersonViewRefComponent>();
            m_healthViewPool = m_world.GetPool<HealthViewRefComponent>();

            AttacheHealthViewComponent();
        }
        
        public void Run(IEcsSystems systems)
        {
            AttacheHealthViewComponent();
        }
        
        private void AttacheHealthViewComponent()
        {
            foreach (var entity in m_healthFilter)
            {
                var spawnPoint = m_heroViewPool.Get(entity).PersonView.GetHealthSpawnPoint();
                var healthView = Object.Instantiate(m_healthViewPrefab, spawnPoint).GetComponent<HealthView>();
                
                healthView.transform.localPosition = Vector3.zero;

                ref HealthViewRefComponent view = ref m_healthViewPool.Add(entity);
                view.HealthView = healthView;
            }
        }
    }
}