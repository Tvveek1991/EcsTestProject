using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float FADE_DURATION = .15f;
        private const float SLIDER_CHANGE_DURATION = .25f;

        private EcsWorld m_world;

        private EcsFilter m_hitHealthViewFilter;
        private EcsFilter m_healHealthViewFilter;

        private EcsPool<HealthComponent> m_healthPool;
        private EcsPool<HealthViewRefComponent> m_healthViewPool;
        private EcsPool<HurtCommandComponent> m_hurtCommandPool;
        private EcsPool<HealCommandComponent> m_healCommandPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitHealthViewFilter = m_world.Filter<HealthComponent>().Inc<HurtCommandComponent>().End();
            m_healHealthViewFilter = m_world.Filter<HealthComponent>().Inc<HealCommandComponent>().End();

            m_healthPool = m_world.GetPool<HealthComponent>();
            m_healthViewPool = m_world.GetPool<HealthViewRefComponent>();
            m_hurtCommandPool = m_world.GetPool<HurtCommandComponent>();
            m_healCommandPool = m_world.GetPool<HealCommandComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            ShowHeal();
            ShowHurt();
        }

        private void ShowHeal()
        {
            foreach (var entity in m_healHealthViewFilter)
            {
                ref HealthComponent healthComponent = ref m_healthPool.Get(entity);
                ref HealthViewRefComponent healthViewRefComponent = ref m_healthViewPool.Get(healthComponent.HealthViewEntityIndex);

                var healthView = healthViewRefComponent.HealthView;
                healthView.HealthBar.DOValue(healthComponent.Health, SLIDER_CHANGE_DURATION)
                    .OnComplete(() =>
                    {
                        if (healthView.HealthBar.value >= healthView.HealthBar.maxValue)
                            healthView.CanvasGroup.DOFade(0f, FADE_DURATION);
                    });

                m_healCommandPool.Del(entity);
            }
        }

        private void ShowHurt()
        {
            foreach (var entity in m_hitHealthViewFilter)
            {
                ref HealthComponent healthComponent = ref m_healthPool.Get(entity);
                ref HealthViewRefComponent healthViewRefComponent = ref m_healthViewPool.Get(healthComponent.HealthViewEntityIndex);

                var healthView = healthViewRefComponent.HealthView;
                if (healthView.CanvasGroup.alpha <= 0)
                    healthView.CanvasGroup.DOFade(1f, FADE_DURATION);

                healthView.HealthBar.DOValue(healthComponent.Health, SLIDER_CHANGE_DURATION)
                    .OnComplete(() =>
                    {
                        if (healthView.HealthBar.value <= 0)
                            healthView.CanvasGroup.DOFade(0f, FADE_DURATION);
                    });

                m_hurtCommandPool.Del(entity);
            }
        }
    }
}