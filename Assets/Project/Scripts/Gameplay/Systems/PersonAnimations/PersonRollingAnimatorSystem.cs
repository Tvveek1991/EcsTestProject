using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonRollingAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_roll = Animator.StringToHash("Roll");
        
        private EcsWorld m_world;
        
        private EcsFilter m_rollingFilter;
        
        private EcsPool<Rolling> m_rollingPool;
        private EcsPool<AnimatorKeeper> m_animatorPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_rollingFilter = m_world.Filter<AnimatorKeeper>().Inc<Rolling>().End();
            
            m_rollingPool = m_world.GetPool<Rolling>();
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckRolling();
        }

        private void CheckRolling()
        {
            foreach (var entity in m_rollingFilter)
            {
                if (!m_rollingPool.Get(entity).IsAnimate) continue;

                m_rollingPool.Get(entity).IsAnimate = false;
                m_animatorPool.Get(entity).AnimatorController.SetTrigger(m_roll);
            }
        }
    }
}