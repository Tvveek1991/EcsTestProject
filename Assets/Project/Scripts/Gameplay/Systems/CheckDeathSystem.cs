using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckDeathSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_healthFilter;
        private EcsFilter m_afterDeadFilter;

        private EcsPool<Dead> m_deadPool;
        private EcsPool<Health> m_healthPool;
        private EcsPool<DeadCommand> m_deadCommandPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_healthFilter = m_world.Filter<Health>()
                .Exc<DeadCommand>().Exc<Dead>().End();
            m_afterDeadFilter = m_world.Filter<Health>().Inc<DeadCommand>()
                .Exc<Dead>().End();

            m_deadPool = m_world.GetPool<Dead>();
            m_healthPool = m_world.GetPool<Health>();
            m_deadCommandPool = m_world.GetPool<DeadCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            AttachDeadComponent();
            CheckDeath();
        }

        private void AttachDeadComponent()
        {
            foreach (var deadEntity in m_afterDeadFilter)
            {
                m_deadPool.Add(deadEntity);
                m_deadCommandPool.Del(deadEntity);
            }
        }

        private void CheckDeath()
        {
            foreach (var healthEntity in m_healthFilter)
            {
                ref Health health = ref m_healthPool.Get(healthEntity);

                if (health.Count <= 0)
                    m_deadCommandPool.Add(healthEntity);
            }
        }
    }
}