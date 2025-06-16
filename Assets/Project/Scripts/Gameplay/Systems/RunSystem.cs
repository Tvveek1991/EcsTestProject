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
        private EcsFilter m_blockFilter;
        private EcsFilter m_inputFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<WallCheckComponent> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;

        private float m_delayToIdle;

        public RunSystem(HeroData heroData)
        {
            m_heroData = heroData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_blockFilter = m_world.Filter<PersonComponent>().Inc<BlockComponent>().Inc<Rigidbody2dComponent>().End();
            m_runFilter = m_world.Filter<PersonComponent>().Inc<Rigidbody2dComponent>().Inc<WallCheckComponent>().Inc<GroundCheckComponent>()
                .Exc<RollingComponent>().Exc<BlockComponent>().End();

            m_runPool = m_world.GetPool<RunComponent>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_wallCheckPool = m_world.GetPool<WallCheckComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var runIndex in m_runFilter)
            foreach (var inputIndex in m_inputFilter)
            {
                if (m_wallCheckPool.Get(runIndex).WallSensors != null)
                    if (m_wallCheckPool.Get(runIndex).WallSensors.Any(item => item.IsConnected) && !m_groundCheckPool.Get(runIndex).GroundSensor.IsConnected)
                        continue;
                
                var inputX = GetDirection(inputIndex);
                m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity = new Vector2(inputX * m_heroData.Speed, m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity.y);

                //Run
                if (Mathf.Abs(inputX) > Mathf.Epsilon)
                {
                    // Reset timer
                    m_delayToIdle = 0.05f;

                    if (!m_runPool.Has(runIndex))
                        m_runPool.Add(runIndex);
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

        private int GetDirection(int inputIndex)
        {
            int inputX;
            if (m_inputPool.Get(inputIndex).IsMoveRightPressed)
                inputX = 1;
            else if (m_inputPool.Get(inputIndex).IsMoveLeftPressed)
                inputX = -1;
            else
                inputX = 0;
            return inputX;
        }
    }
}