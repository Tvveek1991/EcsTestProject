using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class AttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_attackFilter;
        private EcsFilter m_inputFilter;
        
        private EcsPool<AttackComponent> m_attackPool;
        private EcsPool<InputComponent> m_inputPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_attackFilter = m_world.Filter<HeroComponent>().Inc<AttackComponent>().Exc<RollingComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_attackPool = m_world.GetPool<AttackComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var input in m_inputFilter)
            foreach (var personIndex in m_attackFilter)
            {
                ref AttackComponent attackComponent = ref m_attackPool.Get(personIndex);
                attackComponent.TimeSinceAttack += Time.deltaTime;
                
                if (m_inputPool.Get(input).IsAttack && attackComponent.TimeSinceAttack > 0.25f)
                {
                    attackComponent.CurrentAttackIndex++;
                    
                    if (attackComponent.CurrentAttackIndex > 3)
                        attackComponent.CurrentAttackIndex = 1;

                    if (attackComponent.TimeSinceAttack > 1.0f)
                        attackComponent.CurrentAttackIndex = 1;
                    
                    attackComponent.TimeSinceAttack = 0.0f;
                }
            }
        }
    }
}