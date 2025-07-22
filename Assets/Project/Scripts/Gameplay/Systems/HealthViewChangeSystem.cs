using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float FADE_DURATION = .25f;
        private const float SLIDER_CHANGE_DURATION = .5f;

        private EcsWorld m_world;
        
        private EcsFilter m_hitHealthViewFilter;
        private EcsFilter m_healHealthViewFilter;
        
        private EcsPool<HealthViewRefComponent> m_healthViewPool;
        private EcsPool<HitHealthCommandComponent> m_hitHealthCommandPool;
        private EcsPool<HealHealthCommandComponent> m_healHealthCommandPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_hitHealthViewFilter = m_world.Filter<HealthViewRefComponent>().Inc<HitHealthCommandComponent>().End();
            m_healHealthViewFilter = m_world.Filter<HealthViewRefComponent>().Inc<HealHealthCommandComponent>().End();
            
            m_healthViewPool = m_world.GetPool<HealthViewRefComponent>();
            m_hitHealthCommandPool = m_world.GetPool<HitHealthCommandComponent>();
            m_healHealthCommandPool = m_world.GetPool<HealHealthCommandComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckHeal();
            CheckHit();
        }

        private void CheckHeal()
        {
            foreach (var entity in m_healHealthViewFilter)
            {
                ref HealthViewRefComponent healthViewRefComponent = ref m_healthViewPool.Get(entity);
                ref HealHealthCommandComponent addHealthCommandComponent = ref m_healHealthCommandPool.Get(entity);

                var healthView = healthViewRefComponent.HealthView;

                int addHealth = addHealthCommandComponent.AddHealth;
                float targetHealth = healthViewRefComponent.HealthView.HealthBar.value + addHealth;
                healthView.HealthBar.DOValue(targetHealth, SLIDER_CHANGE_DURATION)
                    .OnComplete(() =>
                    {
                        if(healthView.HealthBar.value >= healthView.HealthBar.maxValue)
                            healthView.CanvasGroup.DOFade(0f, FADE_DURATION);
                    });
                
                m_healHealthCommandPool.Del(entity);
            }
        }

        private void CheckHit()
        {
            foreach (var entity in m_hitHealthViewFilter)
            {
                ref HealthViewRefComponent healthViewRefComponent = ref m_healthViewPool.Get(entity);
                ref HitHealthCommandComponent hitHealthCommandComponent = ref m_hitHealthCommandPool.Get(entity);

                healthViewRefComponent.HealthView.CanvasGroup.DOFade(1f, FADE_DURATION);

                int hitDamage = hitHealthCommandComponent.DecreaseHealth;
                float targetHealth = healthViewRefComponent.HealthView.HealthBar.value - hitDamage;
                healthViewRefComponent.HealthView.HealthBar.DOValue(targetHealth, SLIDER_CHANGE_DURATION);
                
                m_hitHealthCommandPool.Del(entity);
            }
        }
    }
}