using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class AttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float TIME_TO_REMOVE_COMPONENT = 2f;
        
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_attackFilter;
        private EcsFilter m_readyToAttackFilter;

        private EcsPool<AttackComponent> m_attackPool;
        private EcsPool<InputComponent> m_inputPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_attackFilter = m_world.Filter<PersonComponent>().Inc<AttackComponent>().Exc<RollingComponent>().Exc<BlockComponent>().End();
            m_readyToAttackFilter = m_world.Filter<PersonComponent>().Exc<AttackComponent>().Exc<RollingComponent>().Exc<BlockComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_attackPool = m_world.GetPool<AttackComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            AttachAttackComponent();
            CheckToStartAttack();
        }

        private void AttachAttackComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var personIndex in m_readyToAttackFilter)
            {
                if (!m_inputPool.Get(input).IsAttack) continue;

                ref AttackComponent attackComponent = ref m_attackPool.Add(personIndex);
                attackComponent.TimeSinceAttack = .3f;
            }
        }

        private void CheckToStartAttack()
        {
            foreach (var input in m_inputFilter)
            foreach (var index in m_attackFilter)
            {
                ref AttackComponent attackComponent = ref m_attackPool.Get(index);
                attackComponent.TimeSinceAttack += Time.deltaTime;

                if (m_inputPool.Get(input).IsAttack && attackComponent.TimeSinceAttack > 0.25f)
                {
                    attackComponent.IsAnimate = true;
                    attackComponent.CurrentAttackIndex++;

                    if (attackComponent.CurrentAttackIndex > 3)
                        attackComponent.CurrentAttackIndex = 1;

                    if (attackComponent.TimeSinceAttack > 1.0f)
                        attackComponent.CurrentAttackIndex = 1;

                    attackComponent.IsAnimate = true;
                    attackComponent.TimeSinceAttack = 0.0f;
                }
                
                if(m_attackPool.Get(index).TimeSinceAttack > TIME_TO_REMOVE_COMPONENT)
                    m_attackPool.Del(index);
            }
        }
    }
}