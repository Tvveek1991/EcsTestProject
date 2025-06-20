using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckPlayerInputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
        
        private EcsFilter m_inputFilter;
        private EcsFilter m_readyToJumpFilter;
        private EcsFilter m_readyToBlockFilter;
        private EcsFilter m_runFilter;
        private EcsFilter m_readyToRunFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<JumpComponent> m_jumpPool;
        private EcsPool<BlockComponent> m_blockPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<WallCheckComponent> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_readyToJumpFilter = m_world.Filter<PersonComponent>().Inc<GroundCheckComponent>()
                .Exc<BlockComponent>().Exc<RollingComponent>().Exc<JumpComponent>().End();
            m_readyToBlockFilter = m_world.Filter<PersonComponent>().Inc<GroundCheckComponent>()
                .Exc<BlockComponent>().Exc<RollingComponent>().End();
            m_runFilter = m_world.Filter<RunComponent>().End();
            m_readyToRunFilter = m_world.Filter<PersonComponent>().Inc<GroundCheckComponent>().Inc<WallCheckComponent>()
                .Exc<RollingComponent>().Exc<BlockComponent>().End();
            
            m_runPool = m_world.GetPool<RunComponent>();
            m_jumpPool = m_world.GetPool<JumpComponent>();
            m_blockPool = m_world.GetPool<BlockComponent>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_wallCheckPool = m_world.GetPool<WallCheckComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            AttachJumpComponent();
            AttachBlockComponent();
            AttachRunComponent();
            CheckRunDirection();
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

        private void AttachRunComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var runIndex in m_readyToRunFilter)
            {
                if (m_wallCheckPool.Get(runIndex).WallSensors != null)
                    if (m_wallCheckPool.Get(runIndex).WallSensors.Any(item => item.IsConnected) && !m_groundCheckPool.Get(runIndex).GroundSensor.IsConnected)
                        continue;

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
    }
}