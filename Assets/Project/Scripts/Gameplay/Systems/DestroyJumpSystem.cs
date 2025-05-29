using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyJumpSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_jumperFilter;
        
        private EcsPool<JumpComponent> m_jumpPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_jumperFilter = m_world.Filter<JumpComponent>().End();

            m_jumpPool = m_world.GetPool<JumpComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var jumper in m_jumperFilter)
            {
                m_jumpPool.Del(jumper);
            }
        }
    }
}