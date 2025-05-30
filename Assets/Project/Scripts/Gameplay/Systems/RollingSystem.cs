using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class RollingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly HeroData m_heroData;

        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_notReadyRollingFilter;

        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<RollingComponent> m_rollingPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<SpriteRendererComponent> m_spriteRendererPool;

        public RollingSystem(HeroData heroData)
        {
            m_heroData = heroData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_notReadyRollingFilter = m_world.Filter<GroundCheckComponent>().Inc<Rigidbody2dComponent>().Inc<SpriteRendererComponent>().Exc<JumpComponent>().Exc<RollingComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_rollingPool = m_world.GetPool<RollingComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var input in m_inputFilter)
            foreach (var withoutRollIndex in m_notReadyRollingFilter)
            {
                if (m_inputPool.Get(input).IsRollPressed && m_groundCheckPool.Get(withoutRollIndex).GroundSensor.IsGrounded)
                {
                    m_rollingPool.Add(withoutRollIndex);
                    
                    int facingDirection = m_spriteRendererPool.Get(withoutRollIndex).SpriteRenderer.flipX ? -1 : 1;
                
                    m_rigidbody2dPool.Get(withoutRollIndex).Rigidbody.linearVelocity = new Vector2(
                        facingDirection * m_heroData.RollForce, m_rigidbody2dPool.Get(withoutRollIndex).Rigidbody.linearVelocity.y);
                }
            }
        }
    }
}