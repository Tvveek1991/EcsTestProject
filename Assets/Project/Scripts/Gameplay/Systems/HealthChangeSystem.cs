using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PersonData m_personData;
        
        private EcsWorld m_world;

        private EcsFilter m_hitHealthViewFilter;
        private EcsFilter m_healHealthViewFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<HitCommand> m_hitCommandPool;
        private EcsPool<HealCommand> m_healCommandPool;

        public HealthChangeSystem(PersonData personData)
        {
            m_personData = personData;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitHealthViewFilter = m_world.Filter<Health>().Inc<HitCommand>().End();
            m_healHealthViewFilter = m_world.Filter<Health>().Inc<HealCommand>().End();

            m_healthPool = m_world.GetPool<Health>();
            m_hitCommandPool = m_world.GetPool<HitCommand>();
            m_healCommandPool = m_world.GetPool<HealCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckHeal();
            CheckHurt();
        }
        
        private void CheckHeal()
        {
            foreach (var entity in m_healHealthViewFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);
                ref HealCommand healCommand = ref m_healCommandPool.Get(entity);

                int maxHealthValue = m_personData.FullHealth;
                int addHealthValue = healCommand.AddHealth;

                health.Count += addHealthValue;
                health.Count = health.Count > maxHealthValue ? maxHealthValue : health.Count;
            }
        }

        private void CheckHurt()
        {
            foreach (var entity in m_hitHealthViewFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);
                ref HitCommand hitCommand = ref m_hitCommandPool.Get(entity);

                int hitDamageValue = hitCommand.HitValue;

                health.Count -= hitDamageValue;
                health.Count = health.Count < 0 ? 0 : health.Count;
            }
        }
    }
}