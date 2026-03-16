using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public class CheckInputMoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_runFilter;
        private EcsFilter m_readyToRunFilter;

        private EcsPool<Run> m_runPool;
        private EcsPool<InputComponent> m_inputPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            m_runFilter = m_world.Filter<Run>().End();
            m_readyToRunFilter = m_world.Filter<Player>().Inc<GroundCheckComponent>().Inc<WallCheck>()
                .Exc<Rolling>().Exc<Block>().End(1);

            m_runPool = m_world.GetPool<Run>();
            m_inputPool = m_world.GetPool<InputComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (!CheckInput())
                return;

            AttachRunComponent();
            CheckRunDirection();
        }

        private void AttachRunComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var runIndex in m_readyToRunFilter)
            {
                bool isInMove = Mathf.Abs(GetDirection(input)) > Mathf.Epsilon;

                if (!m_runPool.Has(runIndex) && isInMove)
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
            if (m_inputPool.Get(inputIndex).IsMoveRight)
                inputX = 1;
            else if (m_inputPool.Get(inputIndex).IsMoveLeft)
                inputX = -1;
            else
                inputX = 0;
            return inputX;
        }

        private bool CheckInput()
        {
            foreach (var inputEntity in m_inputFilter)
                return m_inputPool.Get(inputEntity).IsEnabled;

            return true;
        }
    }
}