using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class BlockSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_blockFilter;

        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<BlockComponent> m_blockPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_blockFilter = m_world.Filter<BlockComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_blockPool = m_world.GetPool<BlockComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            StopMoving();
            DelComponent();
        }

        private void StopMoving()
        {
            foreach (var entity in m_blockFilter)
            {
                m_rigidbody2dPool.Get(entity).Rigidbody.linearVelocity = new Vector2(0, m_rigidbody2dPool.Get(entity).Rigidbody.linearVelocity.y);
            }
        }

        private void DelComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var entity in m_blockFilter)
            {
                if (!m_inputPool.Get(input).IsBlock)
                    m_blockPool.Del(entity);
            }
        }
    }
}