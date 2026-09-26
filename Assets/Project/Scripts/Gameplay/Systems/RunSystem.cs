using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class RunSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float IdleDelay = .05f;

        private readonly PersonData m_personData;

        private EcsWorld m_world;

        private EcsFilter m_runFilter;
        private EcsFilter m_runWithBlockFilter;

        private EcsPool<Run> m_runPool;
        private EcsPool<Rigidbody2d> m_rigidbody2dPool;
        private EcsPool<WallCheck> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public RunSystem(PersonData personData)
        {
            m_personData = personData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_runFilter = m_world.Filter<Run>().Inc<Rigidbody2d>()
                .Exc<Rolling>().Exc<Block>().Exc<Dead>().End();

            m_runPool = m_world.GetPool<Run>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2d>();
            m_wallCheckPool = m_world.GetPool<WallCheck>();
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
                if (m_wallCheckPool.Get(runIndex).WallSensors != null && m_groundCheckPool.Get(runIndex).GroundSensors != null)
                    if (m_wallCheckPool.Get(runIndex).WallSensors.Any(item => item.IsConnected) && m_groundCheckPool.Get(runIndex).GroundSensors.All(item => !item.IsConnected))
                        continue;

                ref Run run = ref m_runPool.Get(runIndex);
                int direction = run.Direction;
                m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity = new Vector2(direction * m_personData.Speed, m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity.y);

                if (Mathf.Abs(direction) > Mathf.Epsilon)
                    run.IdleTimeRemaining = IdleDelay;
            }
        }

        private void CheckRunToRemove()
        {
            foreach (var runIndex in m_runFilter)
            {
                ref Run run = ref m_runPool.Get(runIndex);
                run.IdleTimeRemaining -= Time.deltaTime;

                if (run.IdleTimeRemaining < 0 && m_runPool.Has(runIndex))
                {
                    m_runPool.Del(runIndex);
                    m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity = new Vector2(0, m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity.y);
                }
            }
        }
    }
}
