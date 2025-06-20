using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class RunSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly HeroData m_heroData;

        private EcsWorld m_world;

        private EcsFilter m_runFilter;
        private EcsFilter m_blockFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;

        private float m_delayToIdle;

        public RunSystem(HeroData heroData)
        {
            m_heroData = heroData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_blockFilter = m_world.Filter<BlockComponent>().Inc<Rigidbody2dComponent>().End();
            m_runFilter = m_world.Filter<RunComponent>().Inc<Rigidbody2dComponent>().End();

            m_runPool = m_world.GetPool<RunComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var runIndex in m_runFilter)
            {
                int direction = m_runPool.Get(runIndex).Direction;
                m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity = new Vector2(direction * m_heroData.Speed, m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity.y);

                //Run
                if (Mathf.Abs(direction) > Mathf.Epsilon)
                {
                    // Reset timer
                    m_delayToIdle = 0.05f;
                }
                //Idle
                else
                {
                    // Prevents flickering transitions to idle
                    m_delayToIdle -= Time.deltaTime;
                    if (m_delayToIdle < 0 && m_runPool.Has(runIndex))
                        m_runPool.Del(runIndex);
                }
            }
            
            //if no need a person drift
            StopDrift();
        }

        private void StopDrift()
        {
            foreach (var blockIndex in m_blockFilter)
                m_rigidbody2dPool.Get(blockIndex).Rigidbody.linearVelocity = Vector2.zero;
        }
    }
}