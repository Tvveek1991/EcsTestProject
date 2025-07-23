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
        
        private EcsPool<HealthComponent> m_healthPool;
        private EcsPool<HurtCommandComponent> m_hurtCommandPool;
        private EcsPool<HealCommandComponent> m_healCommandPool;

        public HealthChangeSystem(PersonData personData)
        {
            m_personData = personData;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitHealthViewFilter = m_world.Filter<HealthComponent>().Inc<HurtCommandComponent>().End();
            m_healHealthViewFilter = m_world.Filter<HealthComponent>().Inc<HealCommandComponent>().End();

            m_healthPool = m_world.GetPool<HealthComponent>();
            m_hurtCommandPool = m_world.GetPool<HurtCommandComponent>();
            m_healCommandPool = m_world.GetPool<HealCommandComponent>();
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
                ref HealthComponent healthComponent = ref m_healthPool.Get(entity);
                ref HealCommandComponent healCommandComponent = ref m_healCommandPool.Get(entity);

                int maxHealthValue = m_personData.FullHealth;
                int addHealthValue = healCommandComponent.AddHealth;

                healthComponent.Health += addHealthValue;
                healthComponent.Health = healthComponent.Health > maxHealthValue ? maxHealthValue : healthComponent.Health;

                m_healCommandPool.Del(entity);
            }
        }

        private void CheckHurt()
        {
            foreach (var entity in m_hitHealthViewFilter)
            {
                ref HealthComponent healthComponent = ref m_healthPool.Get(entity);
                ref HurtCommandComponent hurtCommandComponent = ref m_hurtCommandPool.Get(entity);

                int hitDamageValue = hurtCommandComponent.HitValue;

                healthComponent.Health -= hitDamageValue;
                healthComponent.Health = healthComponent.Health < 0 ? 0 : healthComponent.Health;
            }
        }
    }
}