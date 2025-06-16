using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyRollingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_rollingFilter;
        
        private EcsPool<RollingComponent> m_rollingPool;
        
        private float m_rollDuration = .643f;
        private float m_rollCurrentTime;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_rollingFilter = m_world.Filter<RollingComponent>().End();

            m_rollingPool = m_world.GetPool<RollingComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var roller in m_rollingFilter)
            {
                m_rollCurrentTime += Time.deltaTime;
                if (m_rollCurrentTime > m_rollDuration)
                {
                    m_rollCurrentTime = 0.0f;
                    m_rollingPool.Del(roller);
                }
            }
        }
    }
}