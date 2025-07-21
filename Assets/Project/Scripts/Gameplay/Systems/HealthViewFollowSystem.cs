using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewFollowSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly Camera m_camera;
        
        private EcsWorld m_world;
        private EcsFilter m_healthViewFilter;
        private EcsFilter m_personViewWithHealthFilter;

        private EcsPool<HealthComponent> m_healthPool;
        private EcsPool<PersonViewRefComponent> m_personViewPool;
        private EcsPool<HealthViewRefComponent> m_healthViewPool;

        public HealthViewFollowSystem(Camera camera)
        {
            m_camera = camera;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_healthViewFilter = m_world.Filter<HealthViewRefComponent>().End();
            m_personViewWithHealthFilter = m_world.Filter<PersonViewRefComponent>().Inc<HealthComponent>().End();

            m_healthPool = m_world.GetPool<HealthComponent>();
            m_personViewPool = m_world.GetPool<PersonViewRefComponent>();
            m_healthViewPool = m_world.GetPool<HealthViewRefComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            FollowHealthViewToPerson();
        }

        private void FollowHealthViewToPerson()
        {
            foreach (var healthViewEntity in m_healthViewFilter)
            foreach (var personViewWithHealthEntity in m_personViewWithHealthFilter)
            {
                ref HealthComponent healthComponent = ref m_healthPool.Get(personViewWithHealthEntity);
                if (healthViewEntity == healthComponent.ViewEntityIndex)
                {
                    ref PersonViewRefComponent personViewComponent = ref m_personViewPool.Get(personViewWithHealthEntity);
                    ref HealthViewRefComponent healthViewComponent = ref m_healthViewPool.Get(healthViewEntity);

                    Vector3 screenPosition = m_camera.WorldToScreenPoint(personViewComponent.PersonView.GetHealthFollowPoint().position);
                    healthViewComponent.HealthView.GetComponent<RectTransform>().position = new Vector2(screenPosition.x, screenPosition.y);
                }
            }
        }
    }
}