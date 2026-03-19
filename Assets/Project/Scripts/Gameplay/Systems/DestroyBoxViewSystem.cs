using DG.Tweening;
using Gameplay.Services.ObjectsService;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Serializabled;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyBoxViewSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private readonly GameObject m_destroyParticlesPrefab;
        private readonly IObjectsService m_objectsService;

        private EcsWorld m_world;

        private EcsFilter m_deadFilter;
        private EcsFilter m_deadCommandFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<DeadCommand> m_deadCommandPool;
        private EcsPool<TransformKeeper> m_transformPool;

        public DestroyBoxViewSystem(GameObject destroyParticlesPrefab, IObjectsService objectsService)
        {
            m_objectsService = objectsService;
            m_destroyParticlesPrefab = destroyParticlesPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_deadFilter = m_world.Filter<Health>().Inc<Dead>().End();
            m_deadCommandFilter = m_world.Filter<DeadCommand>().Inc<TransformKeeper>().Inc<ObjectViewComponent>().End();

            m_healthPool = m_world.GetPool<Health>();
            m_deadCommandPool = m_world.GetPool<DeadCommand>();
            m_transformPool = m_world.GetPool<TransformKeeper>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in m_deadCommandFilter)
            {
                if(!m_objectsService.Views.TryGetValue(entity, out var view))
                    continue;

                ref var deadCommand = ref m_deadCommandPool.Get(entity);
                if(deadCommand.ObjectViewDestroyedStatus != ProcessStatus.Ready)
                    continue;
                
                deadCommand.ObjectViewDestroyedStatus = ProcessStatus.Started;
                var particles = Object.Instantiate(m_destroyParticlesPrefab, view.GetDestroyParticlesPoint().position, Quaternion.identity, null);

                var objectTransform = m_transformPool.Get(entity).ObjectTransform;
                objectTransform.DOScale(0, .25f)
                    .OnComplete(() =>
                    {
                        objectTransform.DOKill();
                        Object.Destroy(particles.gameObject);

                        m_deadCommandPool.Get(entity).ObjectViewDestroyedStatus = ProcessStatus.Completed;
                    });
            }
        }
        
        public void PostRun(IEcsSystems systems)
        {
            foreach (var entity in m_deadFilter)
            {
                if (!m_objectsService.Views.TryGetValue(entity, out var view))
                    continue;

                m_objectsService.RemoveView(entity);
                Object.Destroy(view.gameObject);
                
                m_world.DelEntity(entity);
            }
        }
    }
}