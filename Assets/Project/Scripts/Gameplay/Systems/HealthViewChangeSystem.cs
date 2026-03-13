using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewChangeSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private const float FADE_DURATION = .15f;
        private const float SLIDER_CHANGE_DURATION = .25f;

        private EcsWorld m_world;

        private EcsFilter m_hitHealthFilter;
        private EcsFilter m_healHealthFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<HealthViewComponent> m_healthViewPool;
        private EcsPool<HurtCommand> m_hurtCommandPool;
        private EcsPool<HealCommand> m_healCommandPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitHealthFilter = m_world.Filter<Health>().Inc<HurtCommand>().End();
            m_healHealthFilter = m_world.Filter<Health>().Inc<HealCommand>().End();

            m_healthPool = m_world.GetPool<Health>();
            m_healthViewPool = m_world.GetPool<HealthViewComponent>();
            m_hurtCommandPool = m_world.GetPool<HurtCommand>();
            m_healCommandPool = m_world.GetPool<HealCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            ShowHeal();
            ShowHurt();
        }
        
        public void PostRun(IEcsSystems systems)
        {
            foreach (var entity in m_healHealthFilter)
            {
                m_healCommandPool.Del(entity);
            }
            
            foreach (var entity in m_hitHealthFilter)
            {
                Health health = m_healthPool.Get(entity);
                int viewEntity = health.ViewEntityIndex;
                
                if (health.Count <= 0)
                {
                    m_world.DelEntity(viewEntity);
                }
                
                m_hurtCommandPool.Del(entity);
            }
        }

        private void ShowHeal()
        {
            foreach (var entity in m_healHealthFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);
                ref HealthViewComponent healthViewComponent = ref m_healthViewPool.Get(health.ViewEntityIndex);

                var healthView = healthViewComponent.HealthView;
                healthView.HealthBar.DOValue(health.Count, SLIDER_CHANGE_DURATION)
                    .OnComplete(() =>
                    {
                        if (healthView.HealthBar.value >= healthView.HealthBar.maxValue)
                            healthView.CanvasGroup.DOFade(0f, FADE_DURATION);
                    });
            }
        }

        private void ShowHurt()
        {
            foreach (var entity in m_hitHealthFilter)
            {
                Health health = m_healthPool.Get(entity);
                ref HealthViewComponent healthViewComponent = ref m_healthViewPool.Get(health.ViewEntityIndex);

                var healthView = healthViewComponent.HealthView;
                if (healthView.CanvasGroup.alpha <= 0)
                    healthView.CanvasGroup.DOFade(1f, FADE_DURATION);

                healthView.HealthBar.DOValue(health.Count, SLIDER_CHANGE_DURATION).OnComplete(() =>
                {
                    if (health.Count <= 0)
                        Object.Destroy(healthView.gameObject);
                });
            }
        }
    }
}