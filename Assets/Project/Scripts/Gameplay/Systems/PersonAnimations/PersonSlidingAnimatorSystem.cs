using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonSlidingAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_wallSliding = Animator.StringToHash("WallSlide");
        
        private EcsWorld m_world;
        
        private EcsFilter m_wallCheckFilter;
        
        private EcsPool<WallCheck> m_wallCheckPool;
        private EcsPool<AnimatorKeeper> m_animatorPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_wallCheckFilter = m_world.Filter<AnimatorKeeper>().Inc<WallCheck>()
                .Exc<DeadCommand>().Exc<Dead>().End();
            
            m_wallCheckPool = m_world.GetPool<WallCheck>();
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckSliding();
        }
        
        private void CheckSliding()
        {
            foreach (var wallEntity in m_wallCheckFilter)
            {
                if (m_wallCheckPool.Get(wallEntity).WallSensors != null)
                    m_animatorPool.Get(wallEntity).AnimatorController.SetBool(m_wallSliding, m_wallCheckPool.Get(wallEntity).WallSensors.Any(item => item.IsConnected));
            }
        }
    }
}