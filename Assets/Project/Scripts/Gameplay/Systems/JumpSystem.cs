using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class JumpSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly HeroData m_heroData;
        
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_jumperFilter;
        private EcsFilter m_withoutJumpFilter;
        
        private EcsPool<JumpComponent> m_jumpPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public JumpSystem(HeroData heroData)
        {
            m_heroData = heroData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_jumperFilter = m_world.Filter<GroundCheckComponent>().Inc<Rigidbody2dComponent>().Inc<JumpComponent>().End();
            m_withoutJumpFilter = m_world.Filter<GroundCheckComponent>().Exc<JumpComponent>().Exc<RollingComponent>().End();

            m_jumpPool = m_world.GetPool<JumpComponent>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var input in m_inputFilter)
            foreach (var withoutJump in m_withoutJumpFilter)
            {
                if (m_inputPool.Get(input).IsJumpPressed && m_groundCheckPool.Get(withoutJump).GroundSensor.IsConnected)
                {
                    m_jumpPool.Add(withoutJump);
                }
            }

            foreach (var jumper in m_jumperFilter)
            {
                if (m_groundCheckPool.Get(jumper).GroundSensor.IsConnected)
                {
                    m_rigidbody2dPool.Get(jumper).Rigidbody.linearVelocity = new Vector2(m_rigidbody2dPool.Get(jumper).Rigidbody.linearVelocity.x, m_heroData.JumpForce);
                    m_groundCheckPool.Get(jumper).GroundSensor.Disable(0.2f);
                }
            }
        }
    }
}