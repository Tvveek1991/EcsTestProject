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
        private EcsFilter m_objectViewWithHealthFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<TransformKeeper> m_transformPool;
        private EcsPool<PersonViewRef> m_personViewPool;
        private EcsPool<ObjectViewRef> m_objectViewPool;

        public HealthViewFollowSystem(Camera camera)
        {
            m_camera = camera;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_healthViewFilter = m_world.Filter<HealthViewRefComponent>().Inc<TransformKeeper>().End();
            m_personViewWithHealthFilter = m_world.Filter<PersonViewRef>().Inc<Health>().End();
            m_objectViewWithHealthFilter = m_world.Filter<ObjectViewRef>().Inc<Health>().End();

            m_healthPool = m_world.GetPool<Health>();
            m_transformPool = m_world.GetPool<TransformKeeper>();
            m_personViewPool = m_world.GetPool<PersonViewRef>();
            m_objectViewPool = m_world.GetPool<ObjectViewRef>();
        }
        
        public void Run(IEcsSystems systems)
        {
            FollowHealthViewToPerson();
            FollowHealthViewToObject();
        }

        private void FollowHealthViewToPerson()
        {
            foreach (var healthViewEntity in m_healthViewFilter)
            foreach (var personViewWithHealthEntity in m_personViewWithHealthFilter)
            {
                ref Health health = ref m_healthPool.Get(personViewWithHealthEntity);
                if (healthViewEntity == health.ViewEntityIndex)
                {
                    ref TransformKeeper transformKeeper = ref m_transformPool.Get(healthViewEntity);
                    ref PersonViewRef personView = ref m_personViewPool.Get(personViewWithHealthEntity);

                    Vector3 screenPosition = m_camera.WorldToScreenPoint(personView.PersonView.GetHealthFollowPoint().position);
                    transformKeeper.ObjectTransform.position = new Vector2(screenPosition.x, screenPosition.y);
                }
            }
        }

        private void FollowHealthViewToObject()
        {
            foreach (var healthViewEntity in m_healthViewFilter)
            foreach (var objectViewWithHealthEntity in m_objectViewWithHealthFilter)
            {
                ref Health health = ref m_healthPool.Get(objectViewWithHealthEntity);
                if (healthViewEntity == health.ViewEntityIndex)
                {
                    ref TransformKeeper transformKeeper = ref m_transformPool.Get(healthViewEntity);
                    ref ObjectViewRef objectView = ref m_objectViewPool.Get(objectViewWithHealthEntity);

                    Vector3 screenPosition = m_camera.WorldToScreenPoint(objectView.ObjectView.GetHealthFollowPoint().position);
                    transformKeeper.ObjectTransform.position = new Vector2(screenPosition.x, screenPosition.y);
                }
            }
        }
    }
}