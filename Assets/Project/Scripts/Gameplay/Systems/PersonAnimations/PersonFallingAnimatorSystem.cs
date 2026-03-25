using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonFallingAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_grounded = Animator.StringToHash("Grounded");
        
        private EcsWorld m_world;
        
        private EcsFilter m_groundCheckFilter;

        private EcsPool<AnimatorKeeper> m_animatorPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_groundCheckFilter = m_world.Filter<AnimatorKeeper>().Inc<GroundCheckComponent>()
                .Exc<DeadCommand>().Exc<Dead>().End();

            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckFalling();
        }
        
        private void CheckFalling()
        {
            foreach (var entity in m_groundCheckFilter)
            {
                if (m_groundCheckPool.Get(entity).GroundSensors.Any(item => item.IsConnected))
                    m_animatorPool.Get(entity).AnimatorController.SetBool(m_grounded, true);

                if (!m_groundCheckPool.Get(entity).GroundSensors.Any(item => item.IsConnected))
                    m_animatorPool.Get(entity).AnimatorController.SetBool(m_grounded, false);
            }
        }
    }
}