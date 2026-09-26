using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public sealed class EndOfFrameCleanupSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_hitCommandFilter;
        private EcsFilter m_healCommandFilter;
        private EcsFilter m_reactionFilter;
        private EcsFilter m_coinsCounterChangeFilter;

        private EcsPool<HitCommand> m_hitCommandPool;
        private EcsPool<HealCommand> m_healCommandPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitCommandFilter = m_world.Filter<HitCommand>().End();
            m_healCommandFilter = m_world.Filter<HealCommand>().End();
            m_reactionFilter = m_world.Filter<ReactionComponent>().End();
            m_coinsCounterChangeFilter = m_world.Filter<CoinsCounterChange>().End();

            m_hitCommandPool = m_world.GetPool<HitCommand>();
            m_healCommandPool = m_world.GetPool<HealCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in m_hitCommandFilter)
                m_hitCommandPool.Del(entity);

            foreach (var entity in m_healCommandFilter)
                m_healCommandPool.Del(entity);

            foreach (var entity in m_reactionFilter)
                m_world.DelEntity(entity);

            foreach (var entity in m_coinsCounterChangeFilter)
                m_world.DelEntity(entity);
        }
    }
}
