using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Serializabled;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonDeadAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_death = Animator.StringToHash("Death");
        
        private EcsWorld m_world;
        
        private EcsFilter m_deadCommandFilter;
        
        private EcsPool<DeadCommand> m_deadCommandPool;
        private EcsPool<AnimatorKeeper> m_animatorPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_deadCommandFilter = m_world.Filter<Person>().Inc<AnimatorKeeper>().Inc<DeadCommand>().End();
            
            m_deadCommandPool = m_world.GetPool<DeadCommand>();
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckDeadAnimation();
        }
        
        private void CheckDeadAnimation()
        {
            foreach (var entity in m_deadCommandFilter)
            {
                ref var deadCommand = ref m_deadCommandPool.Get(entity);

                if (deadCommand.Status != ProcessStatus.Ready)
                    continue;

                deadCommand.Status = ProcessStatus.Started;

                m_animatorPool.Get(entity).AnimatorController.SetTrigger(m_death);

                deadCommand.Status = ProcessStatus.Completed;
            }
        }
    }
}