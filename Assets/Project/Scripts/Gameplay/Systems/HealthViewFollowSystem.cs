using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.CameraService;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewFollowSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly ICameraService m_cameraService;
        private readonly IEntityViewRegistry m_entityViewRegistry;

        private EcsWorld m_world;
        private EcsFilter m_healthOwnerFilter;

        private EcsPool<Health> m_healthPool;

        public HealthViewFollowSystem(ICameraService cameraService, IEntityViewRegistry entityViewRegistry)
        {
            m_cameraService = cameraService;
            m_entityViewRegistry = entityViewRegistry;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_healthOwnerFilter = m_world.Filter<Health>().End();

            m_healthPool = m_world.GetPool<Health>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var healthOwnerEntity in m_healthOwnerFilter)
                FollowHealthView(healthOwnerEntity);
        }

        private void FollowHealthView(int healthOwnerEntity)
        {
            ref Health health = ref m_healthPool.Get(healthOwnerEntity);

            if (!m_entityViewRegistry.TryGet(health.ViewEntity, out HealthView healthView) ||
                !m_entityViewRegistry.TryGet(healthOwnerEntity, out EntityView ownerView))
                return;

            Transform followPoint = GetHealthFollowPoint(ownerView);
            if (followPoint == null)
                return;

            Vector3 screenPosition = m_cameraService.Camera.WorldToScreenPoint(followPoint.position);
            healthView.transform.position = new Vector2(screenPosition.x, screenPosition.y);
        }

        private static Transform GetHealthFollowPoint(EntityView ownerView)
        {
            if (ownerView is PersonView personView)
                return personView.GetHealthFollowPoint();

            if (ownerView is ObjectView objectView)
                return objectView.GetHealthFollowPoint();

            return null;
        }
    }
}
