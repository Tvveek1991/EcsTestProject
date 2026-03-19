using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Serializabled;
using Project.Scripts.Gameplay.Services.HealthViewService;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckDeathSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_waitingDeadFilter;
        private EcsFilter m_afterDeadFilter;

        private EcsPool<Dead> m_deadPool;
        private EcsPool<Health> m_healthPool;
        private EcsPool<DeadCommand> m_deadCommandPool;
        private readonly IHealthViewService m_healthViewService;

        public CheckDeathSystem(IHealthViewService healthViewService)
        {
            m_healthViewService = healthViewService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_waitingDeadFilter = m_world.Filter<Health>()
                .Exc<DeadCommand>().Exc<Dead>().End();
            m_afterDeadFilter = m_world.Filter<Health>().Inc<DeadCommand>()
                .Exc<Dead>().End();

            m_deadPool = m_world.GetPool<Dead>();
            m_healthPool = m_world.GetPool<Health>();
            m_deadCommandPool = m_world.GetPool<DeadCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckDeath();
            AttachDeadComponent();
        }

        private void AttachDeadComponent()
        {
            foreach (var deadEntity in m_afterDeadFilter)
            {
                ref var deadCommand = ref m_deadCommandPool.Get(deadEntity);

                if (deadCommand.Status != ProcessStatus.Completed) 
                    continue;

                m_deadCommandPool.Del(deadEntity);
                m_deadPool.Add(deadEntity);
            }
        }

        private void CheckDeath()
        {
            foreach (var entity in m_waitingDeadFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);

                if (!m_healthViewService.Views.TryGetValue(health.ViewEntity, out var view))
                    continue;
                
                if (view.HealthBar.value <= 0)
                {
                    view.HealthBar.DOKill();
                    
                    ref var deadCommand = ref m_deadCommandPool.Add(entity);
                    deadCommand.Status = ProcessStatus.Ready;
                }
            }
        }
    }
}