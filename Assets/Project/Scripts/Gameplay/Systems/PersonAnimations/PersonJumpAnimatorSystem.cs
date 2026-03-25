using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonJumpAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_jump = Animator.StringToHash("Jump");
        private readonly int m_grounded = Animator.StringToHash("Grounded");
        
        private EcsWorld m_world;
        
        private EcsFilter m_jumperFilter;
        
        private EcsPool<AnimatorKeeper> m_animatorPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_jumperFilter = m_world.Filter<AnimatorKeeper>().Inc<Jump>().End();
            
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckJump();
        }

        private void CheckJump()
        {
            foreach (var entity in m_jumperFilter)
            {
                m_animatorPool.Get(entity).AnimatorController.SetTrigger(m_jump);
                m_animatorPool.Get(entity).AnimatorController.SetBool(m_grounded, false);
            }
        }
    }
}