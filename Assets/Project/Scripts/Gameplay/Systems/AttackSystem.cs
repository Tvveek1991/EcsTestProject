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
        private EcsFilter m_readyToAttackFilter;

        private EcsPool<AttackComponent> m_attackPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_attackFilter = m_world.Filter<PersonComponent>().Inc<AttackComponent>().End();

            m_attackPool = m_world.GetPool<AttackComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckToStartAttack();
        }

        private void CheckToStartAttack()
        {
            foreach (var index in m_attackFilter)
            {
                ref AttackComponent attackComponent = ref m_attackPool.Get(index);
                attackComponent.TimeSinceAttack += Time.deltaTime;

                if (attackComponent.IsAnimate && attackComponent.TimeSinceAttack > 0.25f)
                {
                    attackComponent.CurrentAttackIndex++;

                    if (attackComponent.CurrentAttackIndex > 3)
                        attackComponent.CurrentAttackIndex = 1;

                    if (attackComponent.TimeSinceAttack > 1.0f)
                        attackComponent.CurrentAttackIndex = 1;

                    attackComponent.TimeSinceAttack = 0.0f;
                }
                
                if(m_attackPool.Get(index).TimeSinceAttack > TIME_TO_REMOVE_COMPONENT)
                    m_attackPool.Del(index);
            }
        }
    }
}