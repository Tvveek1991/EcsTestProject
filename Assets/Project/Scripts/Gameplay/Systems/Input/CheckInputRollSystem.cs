using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public class CheckInputRollSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_readyRollingFilter;

        private EcsPool<Rolling> m_rollingPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            m_readyRollingFilter = m_world.Filter<Player>().Inc<GroundCheckComponent>()
                .Exc<Block>().Exc<Jump>().Exc<Rolling>().End(1);

            m_rollingPool = m_world.GetPool<Rolling>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (!CheckInput())
                return;

            AttachRollerComponent();
        }

        private void AttachRollerComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var withoutRollIndex in m_readyRollingFilter)
            {
                if (m_inputPool.Get(input).IsRolling && m_groundCheckPool.Get(withoutRollIndex).GroundSensors.Any(item => item.IsConnected))
                {
                    m_rollingPool.Add(withoutRollIndex).IsAnimate = true;
                }
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