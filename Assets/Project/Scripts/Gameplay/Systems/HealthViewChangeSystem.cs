using DG.Tweening;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Services.TweenRegistry;
using Project.Scripts.Gameplay.Views;

namespace Project.Scripts.Gameplay.Systems
{
    public class HealthViewChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float FADE_DURATION = .15f;
        private const float SLIDER_CHANGE_DURATION = .25f;
        
        private readonly IEntityViewRegistry m_entityViewRegistry;
        private readonly IGameplayTweenRegistry m_gameplayTweenRegistry;

        private EcsWorld m_world;

        private EcsFilter m_hitHealthFilter;
        private EcsFilter m_healHealthFilter;

        private EcsPool<Health> m_healthPool;

        public HealthViewChangeSystem(IEntityViewRegistry entityViewRegistry, IGameplayTweenRegistry gameplayTweenRegistry)
        {
            m_entityViewRegistry = entityViewRegistry;
            m_gameplayTweenRegistry = gameplayTweenRegistry;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitHealthFilter = m_world.Filter<Health>().Inc<HitCommand>().End();
            m_healHealthFilter = m_world.Filter<Health>().Inc<HealCommand>().End();

            m_healthPool = m_world.GetPool<Health>();
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

                m_gameplayTweenRegistry.Track(view.HealthBar.DOValue(health.Count, SLIDER_CHANGE_DURATION))
                    .OnComplete(() =>
                    {
                        m_gameplayTweenRegistry.TryExecute(() =>
                        {
                            if (view.HealthBar.value >= view.HealthBar.maxValue)
                                m_gameplayTweenRegistry.Track(view.CanvasGroup.DOFade(0f, FADE_DURATION));
                        });
                    });
                
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
                    m_gameplayTweenRegistry.Track(view.CanvasGroup.DOFade(1f, FADE_DURATION));

                m_gameplayTweenRegistry.Track(view.HealthBar.DOValue(health.Count, SLIDER_CHANGE_DURATION));
                
            }
        }
    }
}
