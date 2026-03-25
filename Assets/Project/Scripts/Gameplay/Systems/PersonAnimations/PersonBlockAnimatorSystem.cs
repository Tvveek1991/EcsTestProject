using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonBlockAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_block = Animator.StringToHash("Block");
        private readonly int m_blocked = Animator.StringToHash("Blocked");
        private readonly int m_idleBlock = Animator.StringToHash("IdleBlock");
        
        private EcsWorld m_world;
        
        private EcsFilter m_blockFilter;
        private EcsFilter m_outOfBlockFilter;
        
        private EcsPool<Block> m_blockPool;
        private EcsPool<AnimatorKeeper> m_animatorPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_blockFilter = m_world.Filter<AnimatorKeeper>().Inc<Person>().Inc<Block>().End();
            m_outOfBlockFilter = m_world.Filter<AnimatorKeeper>().Inc<Person>()
                .Exc<Block>().End();
            
            m_blockPool = m_world.GetPool<Block>();
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckBlock();
        }

        private void CheckBlock()
        {
            foreach (var entity in m_blockFilter)
            {
                if (!m_blockPool.Get(entity).IsAnimate) continue;

                m_blockPool.Get(entity).IsAnimate = false;
                m_animatorPool.Get(entity).AnimatorController.SetTrigger(m_block);
                m_animatorPool.Get(entity).AnimatorController.SetBool(m_idleBlock, true);
            }

            foreach (var entity in m_outOfBlockFilter)
            {
                m_animatorPool.Get(entity).AnimatorController.SetBool(m_idleBlock, false);
            }
        }
    }
}