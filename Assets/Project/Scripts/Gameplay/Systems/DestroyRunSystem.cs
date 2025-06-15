using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyRunSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_runFilter;
        private EcsPool<RunComponent> m_runPool;
        
        private float m_delayToIdle = 0.0f;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            m_runFilter = m_world.Filter<RunComponent>().End();
            m_runPool = m_world.GetPool<RunComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var runIndex in m_runFilter)
            {
                if(!m_runPool.Get(runIndex).IsRun)
                    m_runPool.Del(runIndex);
            }
        }
    }
}