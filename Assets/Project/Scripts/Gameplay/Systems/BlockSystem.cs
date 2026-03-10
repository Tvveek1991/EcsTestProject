using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class BlockSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_blockFilter;

        private EcsPool<Block> m_blockPool;
        private EcsPool<Rigidbody2d> m_rigidbody2dPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_blockFilter = m_world.Filter<Block>().End();

            m_blockPool = m_world.GetPool<Block>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2d>();
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