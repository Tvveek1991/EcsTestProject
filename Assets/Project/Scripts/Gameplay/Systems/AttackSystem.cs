using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class AttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float TIME_TO_REMOVE_COMPONENT = 2f;
        
        private EcsWorld m_world;

        private EcsFilter m_attackFilter;

        private EcsPool<Attack> m_attackPool;
        private EcsPool<AttackCheckComponent> m_attackCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_attackFilter = m_world.Filter<Person>().Inc<Attack>().End();

            m_attackPool = m_world.GetPool<Attack>();
            m_attackCheckPool = m_world.GetPool<AttackCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckToStartAttack();
        }

        private void CheckToStartAttack()
        {
            foreach (var entity in m_attackFilter)
            {
                ref Attack attack = ref m_attackPool.Get(entity);
                attack.TimeSinceAttack += Time.deltaTime;

                if (attack.IsAnimate && attack.TimeSinceAttack > 0.25f)
                {
                    attack.CurrentAttackIndex++;

                    m_attackCheckPool.Add(entity);

                    if (attack.CurrentAttackIndex > 3)
                        attack.CurrentAttackIndex = 1;

                    if (attack.TimeSinceAttack > 1.0f)
                        attack.CurrentAttackIndex = 1;

                    attack.TimeSinceAttack = 0.0f;
                }
                
                if(m_attackPool.Get(entity).TimeSinceAttack > TIME_TO_REMOVE_COMPONENT)
                    m_attackPool.Del(entity);
            }
        }
    }
}