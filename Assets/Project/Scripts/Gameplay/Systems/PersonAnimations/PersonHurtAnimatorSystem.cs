using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonHurtAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_hurt = Animator.StringToHash("Hurt");
        
        private EcsWorld m_world;
        
        private EcsFilter m_hitCommandFilter;
        
        private EcsPool<AnimatorKeeper> m_animatorPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_hitCommandFilter = m_world.Filter<AnimatorKeeper>().Inc<HitCommand>()
                .Exc<DeadCommand>().Exc<Dead>().End();

            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckHurtAnimation();
        }

        private void CheckHurtAnimation()
        {
            foreach (var entity in m_hitCommandFilter)
            {
                m_animatorPool.Get(entity).AnimatorController.SetTrigger(m_hurt);
            }
        }
    }
}