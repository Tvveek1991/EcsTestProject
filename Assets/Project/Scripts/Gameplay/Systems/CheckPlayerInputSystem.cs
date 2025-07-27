using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckPlayerInputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_inputFilter;
        private EcsFilter m_hitFilter;
        private EcsFilter m_readyToJumpFilter;
        private EcsFilter m_blockFilter;
        private EcsFilter m_readyToBlockFilter;
        private EcsFilter m_attackFilter;
        private EcsFilter m_readyToAttackFilter;
        private EcsFilter m_runFilter;
        private EcsFilter m_readyToRunFilter;
        private EcsFilter m_readyRollingFilter;

        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<RunComponent> m_runPool;
        private EcsPool<JumpComponent> m_jumpPool;
        private EcsPool<BlockComponent> m_blockPool;
        private EcsPool<AttackComponent> m_attackPool;
        private EcsPool<RollingComponent> m_rollingPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<HurtCommandComponent> m_hurtCommandPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();

            m_readyToJumpFilter = m_world.Filter<PlayerComponent>().Inc<GroundCheckComponent>()
                .Exc<BlockComponent>().Exc<RollingComponent>().Exc<JumpComponent>().End();
            
            m_blockFilter = m_world.Filter<BlockComponent>().End();
            m_readyToBlockFilter = m_world.Filter<PlayerComponent>().Inc<GroundCheckComponent>()
                .Exc<BlockComponent>().Exc<RollingComponent>().End();
            
            m_runFilter = m_world.Filter<RunComponent>().End();
            m_readyToRunFilter = m_world.Filter<PlayerComponent>().Inc<GroundCheckComponent>().Inc<WallCheckComponent>()
                .Exc<RollingComponent>().Exc<BlockComponent>().End();
            
            m_attackFilter = m_world.Filter<AttackComponent>().End();
            m_readyToAttackFilter = m_world.Filter<PlayerComponent>()
                .Exc<AttackComponent>().Exc<RollingComponent>().Exc<BlockComponent>().End();
            
            m_readyRollingFilter = m_world.Filter<PlayerComponent>().Inc<GroundCheckComponent>()
                .Exc<BlockComponent>().Exc<JumpComponent>().Exc<RollingComponent>().End();
            
            m_hitFilter = m_world.Filter<PlayerComponent>().Inc<HealthComponent>()
                .Exc<HurtCommandComponent>().End();
            
            m_runPool = m_world.GetPool<RunComponent>();
            m_jumpPool = m_world.GetPool<JumpComponent>();
            m_blockPool = m_world.GetPool<BlockComponent>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_attackPool = m_world.GetPool<AttackComponent>();
            m_rollingPool = m_world.GetPool<RollingComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
            m_hurtCommandPool = m_world.GetPool<HurtCommandComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!CheckInput())
                return;
            
            AttachJumpComponent();
            AttachBlockComponent();
            CheckUnblock();
            AttachRunComponent();
            CheckRunDirection();
            AttachAttackComponent();
            CheckAttack();
            AttachRollerComponent();
            
            AttachHurtComponent();
        }

        private bool CheckInput()
        {
            foreach (var inputEntity in m_inputFilter)
                return m_inputPool.Get(inputEntity).IsEnabled;

            return true;
        }

        private void AttachHurtComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var hitEntity in m_hitFilter)
            {
                if (m_inputPool.Get(input).IsHurt) 
                    m_hurtCommandPool.Add(hitEntity).HitValue = 25;
            }
        }
        
        private void AttachJumpComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var readyToJumpEntity in m_readyToJumpFilter)
            {
                if (m_inputPool.Get(input).IsJumpPressed && m_groundCheckPool.Get(readyToJumpEntity).GroundSensor.IsConnected) 
                    m_jumpPool.Add(readyToJumpEntity);
            }
        }

        private void AttachBlockComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var readyToBlockEntity in m_readyToBlockFilter)
            {
                if (m_inputPool.Get(input).IsBlock && m_groundCheckPool.Get(readyToBlockEntity).GroundSensor.IsConnected)
                    m_blockPool.Add(readyToBlockEntity).IsAnimate = true;
            }
        }

        private void CheckUnblock()
        {
            foreach (var input in m_inputFilter)
            foreach (var entity in m_blockFilter)
            {
                if (!m_inputPool.Get(input).IsBlock)
                    m_blockPool.Del(entity);
            }
        }

        private void AttachRunComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var runIndex in m_readyToRunFilter)
            {
                bool isInMove = Mathf.Abs(GetDirection(input)) > Mathf.Epsilon;
                
                if(!m_runPool.Has(runIndex) && isInMove)
                    m_runPool.Add(runIndex);
            }
        }

        private void CheckRunDirection()
        {
            foreach (var input in m_inputFilter)
            foreach (var runIndex in m_runFilter)
            {
                m_runPool.Get(runIndex).Direction = GetDirection(input);
            }
        }
        
        private int GetDirection(int inputIndex)
        {
            int inputX;
            if (m_inputPool.Get(inputIndex).IsMoveRightPressed)
                inputX = 1;
            else if (m_inputPool.Get(inputIndex).IsMoveLeftPressed)
                inputX = -1;
            else
                inputX = 0;
            return inputX;
        }
        
        private void AttachAttackComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var personIndex in m_readyToAttackFilter)
            {
                if (!m_inputPool.Get(input).IsAttack) continue;

                m_attackPool.Add(personIndex);
            }
        }

        private void CheckAttack()
        {
            foreach (var input in m_inputFilter)
            foreach (var entity in m_attackFilter)
            {
                if (!m_inputPool.Get(input).IsAttack) continue;

                ref AttackComponent attackComponent = ref m_attackPool.Get(entity);
                attackComponent.IsAnimate = true;
                attackComponent.TimeSinceAttack = .3f;
            }
        }

        private void AttachRollerComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var withoutRollIndex in m_readyRollingFilter)
            {
                if (m_inputPool.Get(input).IsRollPressed && m_groundCheckPool.Get(withoutRollIndex).GroundSensor.IsConnected)
                {
                    m_rollingPool.Add(withoutRollIndex).IsAnimate = true;
                }
            }
        }
    }
}