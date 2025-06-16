using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyAttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float TIME_TO_REMOVE_COMPONENT = 2f;
        
        private EcsWorld m_world;

        private EcsFilter m_attackFilter;
        
        private EcsPool<AttackComponent> m_attackPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_attackFilter = m_world.Filter<AttackComponent>().End();

            m_attackPool = m_world.GetPool<AttackComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var attackIndex in m_attackFilter)
            {
                if(m_attackPool.Get(attackIndex).TimeSinceAttack > TIME_TO_REMOVE_COMPONENT)
                    m_attackPool.Del(attackIndex);
            }
        }
    }
}