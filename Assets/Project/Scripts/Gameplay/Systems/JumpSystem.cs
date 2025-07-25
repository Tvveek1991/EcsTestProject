using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class JumpSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly PersonData m_personData;

        private EcsWorld m_world;

        private EcsFilter m_jumperFilter;

        // private EcsPool<JumpComponent> m_jumpPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public JumpSystem(PersonData personData)
        {
            m_personData = personData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_jumperFilter = m_world.Filter<JumpComponent>().Inc<Rigidbody2dComponent>().Inc<GroundCheckComponent>().End();

            // m_jumpPool = m_world.GetPool<JumpComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            TryJump();
        }

        private void TryJump()
        {
            foreach (var jumper in m_jumperFilter)
            {
                m_rigidbody2dPool.Get(jumper).Rigidbody.linearVelocity = new Vector2(m_rigidbody2dPool.Get(jumper).Rigidbody.linearVelocity.x, m_personData.JumpForce);
                m_groundCheckPool.Get(jumper).GroundSensor.Disable(0.2f);
            }
        }
    }
}