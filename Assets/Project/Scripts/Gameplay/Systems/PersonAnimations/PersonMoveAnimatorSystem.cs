using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonMoveAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_animState = Animator.StringToHash("Move");
        
        private EcsWorld m_world;
        
        private EcsFilter m_runFilter;
        private EcsFilter m_outOfRunFilter;
        
        private EcsPool<AnimatorKeeper> m_animatorPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_runFilter = m_world.Filter<AnimatorKeeper>().Inc<Run>().End();
            m_outOfRunFilter = m_world.Filter<AnimatorKeeper>()
                .Exc<Run>().End();
            
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckRun();
        }
        
        private void CheckRun()
        {
            foreach (var entity in m_runFilter)
                m_animatorPool.Get(entity).AnimatorController.SetBool(m_animState, true);

            foreach (var entity in m_outOfRunFilter)
                m_animatorPool.Get(entity).AnimatorController.SetBool(m_animState, false);
        }
    }
}