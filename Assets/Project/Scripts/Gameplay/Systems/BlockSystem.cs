using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class BlockSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_blockFilter;

        private EcsPool<BlockComponent> m_blockPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_blockFilter = m_world.Filter<BlockComponent>()
                .Exc<DeadCommandComponent>().Exc<DeadComponent>().End();

            m_blockPool = m_world.GetPool<BlockComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            StopMoving();
        }

        private void StopMoving()
        {
            foreach (var entity in m_blockFilter)
            {
                m_rigidbody2dPool.Get(entity).Rigidbody.linearVelocity = new Vector2(0, m_rigidbody2dPool.Get(entity).Rigidbody.linearVelocity.y);
            }
        }
    }
}