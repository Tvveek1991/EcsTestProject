using DG.Tweening;
using Gameplay.Services.ObjectsService;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckDestroyedParticlesSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private readonly GameObject m_destroyedParticlesPrefab;
        private readonly IObjectsService m_objectsService;

        private EcsWorld m_world;

        private EcsFilter m_filter;

        private EcsPool<TransformKeeper> m_transformPool;

        public CheckDestroyedParticlesSystem(GameObject destroyedParticles, IObjectsService objectsService)
        {
            m_objectsService = objectsService;
            m_destroyedParticlesPrefab = destroyedParticles;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_filter = m_world.Filter<DeadCommand>().Inc<TransformKeeper>().Inc<ObjectViewComponent>().End();

            m_transformPool = m_world.GetPool<TransformKeeper>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in m_filter)
            {
                if(!m_objectsService.Views.TryGetValue(entity, out var view))
                    continue;
                
                var particles = Object.Instantiate(m_destroyedParticlesPrefab, view.GetDestroyParticlesPoint().position, Quaternion.identity, null);

                var objectTransform = m_transformPool.Get(entity).ObjectTransform;
                objectTransform.DOScale(0, .25f)
                    .OnComplete(() =>
                    {
                        objectTransform.DOKill();
                        Object.Destroy(particles.gameObject);
                        Object.Destroy(view.gameObject);
                        m_objectsService.RemoveView(entity);
                    });
            }
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (var entity in m_filter)
            {
                m_world.DelEntity(entity);
            }
        }
    }
}