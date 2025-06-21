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

        private EcsFilter m_rollingFilter;

        private EcsPool<RollingComponent> m_rollingPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<SpriteRendererComponent> m_spriteRendererPool;
        
        private float m_rollDuration = .643f;
        private float m_rollCurrentTime;
        
        public RollingSystem(HeroData heroData)
        {
            m_heroData = heroData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_rollingFilter = m_world.Filter<RollingComponent>().Inc<Rigidbody2dComponent>().Inc<SpriteRendererComponent>().End();

            m_rollingPool = m_world.GetPool<RollingComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            RollPerson();
            DeleteRollerComponent();
        }

        private void RollPerson()
        {
            foreach (var roller in m_rollingFilter)
            {
                if (m_rollingPool.Get(roller).IsAnimate)
                {
                    var facingDirection = m_spriteRendererPool.Get(roller).SpriteRenderer.flipX ? -1 : 1;
                    
                    m_rigidbody2dPool.Get(roller).Rigidbody.linearVelocity = new Vector2(
                        facingDirection * m_heroData.RollForce, m_rigidbody2dPool.Get(roller).Rigidbody.linearVelocity.y);
                }
            }
        }

        private void DeleteRollerComponent()
        {
            foreach (var roller in m_rollingFilter)
            {
                m_rollCurrentTime += Time.deltaTime;
                if (m_rollCurrentTime > m_rollDuration)
                {
                    m_rollCurrentTime = 0.0f;
                    m_rollingPool.Del(roller);
                    
                    m_rigidbody2dPool.Get(roller).Rigidbody.linearVelocity = Vector2.zero;
                }
            }
        }
    }
}