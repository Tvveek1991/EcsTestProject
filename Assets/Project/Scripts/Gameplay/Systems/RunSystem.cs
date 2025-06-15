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
        private EcsFilter m_inputFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;

        private float m_delayToIdle = 0.0f;

        public RunSystem(HeroData heroData)
        {
            m_heroData = heroData;
        }

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_runFilter = m_world.Filter<HeroComponent>().Inc<Rigidbody2dComponent>().Exc<RollingComponent>().End();

            m_runPool = m_world.GetPool<RunComponent>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var inputIndex in m_inputFilter)
            foreach (var runIndex in m_runFilter)
            {
                int inputX = 0;
                if (m_inputPool.Get(inputIndex).IsMoveRightPressed && !m_inputPool.Get(inputIndex).IsBlock)
                    inputX = 1;
                else if (m_inputPool.Get(inputIndex).IsMoveLeftPressed && !m_inputPool.Get(inputIndex).IsBlock)
                    inputX = -1;
                else
                    inputX = 0;

                m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity = new Vector2(inputX * m_heroData.Speed, m_rigidbody2dPool.Get(runIndex).Rigidbody.linearVelocity.y);

                if (inputX != 0 && !m_runPool.Has(runIndex)) 
                    m_runPool.Add(runIndex);

                if (m_runPool.Has(runIndex))
                {
                    //Run
                    if (Mathf.Abs(inputX) > Mathf.Epsilon)
                    {
                        // Reset timer
                        m_delayToIdle = 0.05f;
                        m_runPool.Get(runIndex).IsRun = true;
                    }
                    //Idle
                    else
                    {
                        // Prevents flickering transitions to idle
                        m_delayToIdle -= Time.deltaTime;
                        if (m_delayToIdle < 0)
                            m_runPool.Get(runIndex).IsRun = false;
                    }
                }
                
            }
        }
    }
}