using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Serializabled;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyObjectViewSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private readonly GameObject m_destroyParticlesPrefab;
        private readonly IEntityViewRegistry m_entityViewRegistry;

        private EcsWorld m_world;

        private EcsFilter m_deadFilter;
        private EcsFilter m_deadCommandFilter;

        private EcsPool<DeadCommand> m_deadCommandPool;
        private EcsPool<TransformKeeper> m_transformPool;

        public DestroyObjectViewSystem(GameObject destroyParticlesPrefab, IEntityViewRegistry entityViewRegistry)
        {
            m_entityViewRegistry = entityViewRegistry;
            m_destroyParticlesPrefab = destroyParticlesPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_deadFilter = m_world.Filter<Health>().Inc<Dead>().End();
            m_deadCommandFilter = m_world.Filter<DeadCommand>().Inc<TransformKeeper>().Inc<ObjectViewComponent>().End();

            m_deadCommandPool = m_world.GetPool<DeadCommand>();
            m_transformPool = m_world.GetPool<TransformKeeper>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in m_deadCommandFilter)
            {
                if(!m_entityViewRegistry.TryGet(entity, out ObjectView view))
                    continue;

                ref var deadCommand = ref m_deadCommandPool.Get(entity);
                if(deadCommand.Status != ProcessStatus.Ready)
                    continue;
                
                deadCommand.Status = ProcessStatus.Started;
                var particles = Object.Instantiate(m_destroyParticlesPrefab, view.GetDestroyParticlesPoint().position, Quaternion.identity, null);

                var objectTransform = m_transformPool.Get(entity).ObjectTransform;
                objectTransform.DOScale(0, .25f)
                    .OnComplete(() =>
                    {
                        objectTransform.DOKill();
                        Object.Destroy(particles.gameObject);

                        m_deadCommandPool.Get(entity).Status = ProcessStatus.Completed;
                    });
            }
        }
        
        public void PostRun(IEcsSystems systems)
        {
            foreach (var entity in m_deadFilter)
            {
                if (!m_entityViewRegistry.TryGet(entity, out ObjectView view))
                    continue;

                m_entityViewRegistry.Unregister(entity);
                Object.Destroy(view.gameObject);
                
                m_world.DelEntity(entity);
            }
        }
    }
}
