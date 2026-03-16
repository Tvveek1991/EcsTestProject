using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;
using Project.Scripts.Gameplay.Data;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public class CheckInputAttackSystem: IEcsInitSystem, IEcsRunSystem
    {
        private readonly PersonData m_personData;
        private EcsWorld m_world;
    
        private EcsFilter m_inputFilter;
        private EcsFilter m_attackFilter;
        private EcsFilter m_readyToAttackFilter;
        
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<Attack> m_attackPool;

        public CheckInputAttackSystem(PersonData personData)
        {
            m_personData = personData;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            m_attackFilter = m_world.Filter<Attack>().End(); 
            m_readyToAttackFilter = m_world.Filter<Player>()
                .Exc<Attack>().Exc<Rolling>().Exc<Block>().End(1);

            m_attackPool = m_world.GetPool<Attack>();
            m_inputPool = m_world.GetPool<InputComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!CheckInput())
                return;
            
            AttachAttackComponent();
            
            CheckAttack();
        }
        
        private void AttachAttackComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var entity in m_readyToAttackFilter)
            {
                if (!m_inputPool.Get(input).IsAttack) continue;

                ref var attack = ref m_attackPool.Add(entity);
                    
                attack.IsActive = true;
                attack.TimeSinceAttack = m_personData.TimeToRepeatAttack;
            }
        }

        private void CheckAttack()
        {
            foreach (var input in m_inputFilter)
            foreach (var entity in m_attackFilter)
            {
                if (!m_inputPool.Get(input).IsAttack) 
                    continue;

                ref Attack attack = ref m_attackPool.Get(entity);

                if (attack.TimeSinceAttack < m_personData.TimeToRepeatAttack)
                    continue;
                
                attack.IsActive = true;
            }
        }
        
        private bool CheckInput()
        {
            foreach (var inputEntity in m_inputFilter)
                return m_inputPool.Get(inputEntity).IsEnabled;

            return true;
        }
        
    }
}