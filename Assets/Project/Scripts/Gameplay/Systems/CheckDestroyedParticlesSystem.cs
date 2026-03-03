using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckDestroyedParticlesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly GameObject m_destroyedParticlesPrefab;

        private EcsWorld m_world;
        
        private EcsFilter m_filter;

        private EcsPool<TransformComponent> m_transformPool;
        private EcsPool<ObjectViewRefComponent> m_objectViewRefPool;

        public CheckDestroyedParticlesSystem(GameObject destroyedParticles)
        {
            m_destroyedParticlesPrefab = destroyedParticles;
        } 
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_filter = m_world.Filter<DeadCommandComponent>().Inc<TransformComponent>().Inc<ObjectViewRefComponent>().End();

            m_transformPool = m_world.GetPool<TransformComponent>();
            m_objectViewRefPool = m_world.GetPool<ObjectViewRefComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in m_filter)
            {
                var view = m_objectViewRefPool.Get(entity).ObjectView;
                var particles = Object.Instantiate(m_destroyedParticlesPrefab, view.GetDestroyParticlesPoint().position, Quaternion.identity, null);

                var objectTransform = m_transformPool.Get(entity).ObjectTransform;
                objectTransform.DOScale(0, .25f)
                    .OnComplete(() =>
                    {
                        objectTransform.DOKill();
                        m_world.DelEntity(entity);
                        Object.Destroy(particles.gameObject);
                        Object.Destroy(objectTransform.gameObject);
                    });
            }
        }
    }
}
