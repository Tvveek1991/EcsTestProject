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

        private EcsPool<AttackComponent> m_attackPool;
        private EcsPool<AttackCheckComponent> m_attackCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_attackFilter = m_world.Filter<PersonComponent>().Inc<AttackComponent>()
                .Exc<DeadCommandComponent>().Exc<DeadComponent>().End();

            m_attackPool = m_world.GetPool<AttackComponent>();
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
                ref AttackComponent attackComponent = ref m_attackPool.Get(entity);
                attackComponent.TimeSinceAttack += Time.deltaTime;

                if (attackComponent.IsAnimate && attackComponent.TimeSinceAttack > 0.25f)
                {
                    attackComponent.CurrentAttackIndex++;

                    m_attackCheckPool.Add(entity);

                    if (attackComponent.CurrentAttackIndex > 3)
                        attackComponent.CurrentAttackIndex = 1;

                    if (attackComponent.TimeSinceAttack > 1.0f)
                        attackComponent.CurrentAttackIndex = 1;

                    attackComponent.TimeSinceAttack = 0.0f;
                }
                
                if(m_attackPool.Get(entity).TimeSinceAttack > TIME_TO_REMOVE_COMPONENT)
                    m_attackPool.Del(entity);
            }
        }
    }
}