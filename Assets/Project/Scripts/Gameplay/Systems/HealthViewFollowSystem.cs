using Gameplay.Services.ObjectsService;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CameraService;
using Project.Scripts.Gameplay.Services.HealthViewService;
using Project.Scripts.Gameplay.Services.PersonService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewFollowSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly ICameraService m_cameraService;
        private readonly IPersonViewService m_personViewService;
        private readonly IObjectsService m_objectsService;
        private readonly IHealthViewService m_healthViewService;

        private EcsWorld m_world;
        private EcsFilter m_healthViewFilter;
        private EcsFilter m_personViewWithHealthFilter;
        private EcsFilter m_objectViewWithHealthFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<TransformKeeper> m_transformPool;

        public HealthViewFollowSystem(ICameraService cameraService, IPersonViewService personViewService, IObjectsService objectsService, IHealthViewService healthViewService)
        {
            m_cameraService = cameraService;
            m_objectsService = objectsService;
            m_healthViewService = healthViewService;
            m_personViewService = personViewService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_healthViewFilter = m_world.Filter<HealthViewComponent>().Inc<TransformKeeper>().End();
            m_personViewWithHealthFilter = m_world.Filter<PersonViewComponent>().Inc<Health>().End();
            m_objectViewWithHealthFilter = m_world.Filter<ObjectViewComponent>().Inc<Health>().End();

            m_healthPool = m_world.GetPool<Health>();
            m_transformPool = m_world.GetPool<TransformKeeper>();
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
                if (healthViewEntity == health.ViewEntity)
                {
                    ref TransformKeeper transformKeeper = ref m_transformPool.Get(healthViewEntity);

                    if (!m_personViewService.Views.TryGetValue(personViewWithHealthEntity, out var view)) 
                        continue;

                    Vector3 screenPosition = m_cameraService.Camera.WorldToScreenPoint(view.GetHealthFollowPoint().position);
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
                if (healthViewEntity == health.ViewEntity)
                {
                    ref TransformKeeper transformKeeper = ref m_transformPool.Get(healthViewEntity);
                    
                    if (!m_objectsService.Views.TryGetValue(objectViewWithHealthEntity, out var view)) 
                        continue;

                    Vector3 screenPosition = m_cameraService.Camera.WorldToScreenPoint(view.GetHealthFollowPoint().position);
                    transformKeeper.ObjectTransform.position = new Vector2(screenPosition.x, screenPosition.y);
                }
            }
        }
    }
}