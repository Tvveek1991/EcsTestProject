using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonAttackAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const string ATTACK_KEY = "Attack";
        
        private EcsWorld m_world;
        
        private EcsFilter m_attackFilter;
        
        private EcsPool<Attack> m_attackPool;
        private EcsPool<AnimatorKeeper> m_animatorPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_attackFilter = m_world.Filter<AnimatorKeeper>().Inc<Person>().Inc<Attack>()
                .Exc<Rolling>().End();
            
            m_attackPool = m_world.GetPool<Attack>();
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
        }
        
        public void Run(IEcsSystems systems)
        {
            CheckAttack();
        }
        
        private void CheckAttack()
        {
            foreach (var personEntity in m_attackFilter)
            {
                if (!m_attackPool.Get(personEntity).IsActive) continue;

                m_animatorPool.Get(personEntity).AnimatorController.SetTrigger($"{ATTACK_KEY}{m_attackPool.Get(personEntity).CurrentAttackIndex}");
                m_attackPool.Get(personEntity).IsActive = false;
            }
        }
    }
}