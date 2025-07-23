using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckDeathSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_healthFilter;
        private EcsFilter m_afterDeadFilter;

        private EcsPool<DeadComponent> m_deadPool;
        private EcsPool<HealthComponent> m_healthPool;
        private EcsPool<DeadCommandComponent> m_deadCommandPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_healthFilter = m_world.Filter<HealthComponent>()
                .Exc<DeadCommandComponent>().Exc<DeadComponent>().End();
            m_afterDeadFilter = m_world.Filter<HealthComponent>().Inc<DeadCommandComponent>()
                .Exc<DeadComponent>().End();

            m_deadPool = m_world.GetPool<DeadComponent>();
            m_healthPool = m_world.GetPool<HealthComponent>();
            m_deadCommandPool = m_world.GetPool<DeadCommandComponent>();
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
                ref HealthComponent healthComponent = ref m_healthPool.Get(healthEntity);

                if (healthComponent.Health <= 0)
                    m_deadCommandPool.Add(healthEntity);
            }
        }
    }
}