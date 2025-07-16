using System.Linq;
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
        private EcsFilter m_runWithBlockFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;
        private EcsPool<WallCheckComponent> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        private float m_delayToIdle;

        public RunSystem(HeroData heroData)
        {
            m_heroData = heroData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_runFilter = m_world.Filter<RunComponent>().Inc<Rigidbody2dComponent>().Exc<RollingComponent>().Exc<BlockComponent>().End();

            m_runPool = m_world.GetPool<RunComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
            m_wallCheckPool = m_world.GetPool<WallCheckComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            TryMove();
            CheckRunToRemove();
        }

        private void TryMove()
        {
            foreach (var runIndex in m_runFilter)
            {
                if (m_wallCheckPool.Get(runIndex).WallSensors != null)
                    if (m_wallCheckPool.Get(runIndex).WallSensors.Any(item => item.IsConnected) && !m_groundCheckPool.Get(runIndex).GroundSensor.IsConnected)
                        continue;

                int direction = m_runPool.Get(runIndex).Direction;
                m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity = new Vector2(direction * m_heroData.Speed, m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity.y);

                //Run
                if (Mathf.Abs(direction) > Mathf.Epsilon)
                    m_delayToIdle = 0.05f;
            }
        }

        private void CheckRunToRemove()
        {
            foreach (var runIndex in m_runFilter)
            {
                m_delayToIdle -= Time.deltaTime;
                if (m_delayToIdle < 0 && m_runPool.Has(runIndex))
                {
                    m_runPool.Del(runIndex);
                    m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity = new Vector2(0, m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity.y);
                }
            }
        }
    }
}