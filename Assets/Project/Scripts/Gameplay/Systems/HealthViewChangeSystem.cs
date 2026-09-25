using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float FADE_DURATION = .15f;
        private const float SLIDER_CHANGE_DURATION = .25f;
        
        private readonly IEntityViewRegistry m_entityViewRegistry;

        private EcsWorld m_world;

        private EcsFilter m_hitHealthFilter;
        private EcsFilter m_healHealthFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<HitCommand> m_hitCommandPool;
        private EcsPool<HealCommand> m_healCommandPool;

        public HealthViewChangeSystem(IEntityViewRegistry entityViewRegistry)
        {
            m_entityViewRegistry = entityViewRegistry;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitHealthFilter = m_world.Filter<Health>().Inc<HitCommand>().End();
            m_healHealthFilter = m_world.Filter<Health>().Inc<HealCommand>().End();

            m_healthPool = m_world.GetPool<Health>();
            m_hitCommandPool = m_world.GetPool<HitCommand>();
            m_healCommandPool = m_world.GetPool<HealCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            ShowHeal();
            ShowHurt();
        }

        private void ShowHeal()
        {
            foreach (var entity in m_healHealthFilter)
            {
                ref Health health = ref m_healthPool.Get(entity);
                
                if(!m_entityViewRegistry.TryGet(health.ViewEntity, out HealthView view))
                    continue;

                view.HealthBar.DOValue(health.Count, SLIDER_CHANGE_DURATION)
                    .OnComplete(() =>
                    {
                        if (view.HealthBar.value >= view.HealthBar.maxValue)
                            view.CanvasGroup.DOFade(0f, FADE_DURATION);
                    });
                
                m_healCommandPool.Del(entity);
            }
        }

        private void ShowHurt()
        {
            foreach (var entity in m_hitHealthFilter)
            {
                Health health = m_healthPool.Get(entity);
                
                if(!m_entityViewRegistry.TryGet(health.ViewEntity, out HealthView view))
                    continue;

                if (view.CanvasGroup.alpha <= 0)
                    view.CanvasGroup.DOFade(1f, FADE_DURATION);

                view.HealthBar.DOValue(health.Count, SLIDER_CHANGE_DURATION);
                
                m_hitCommandPool.Del(entity);
            }
        }
    }
}
