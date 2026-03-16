using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public class CheckInputJumpSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_readyToJumpFilter;

        private EcsPool<Jump> m_jumpPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            m_readyToJumpFilter = m_world.Filter<Player>().Inc<GroundCheckComponent>()
                .Exc<Block>().Exc<Rolling>().Exc<Jump>().End(1);

            m_jumpPool = m_world.GetPool<Jump>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (!CheckInput())
                return;

            AttachJumpComponent();
        }

        private void AttachJumpComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var readyToJumpEntity in m_readyToJumpFilter)
            {
                if (m_inputPool.Get(input).IsJump && m_groundCheckPool.Get(readyToJumpEntity).GroundSensors.Any(item => item.IsConnected))
                    m_jumpPool.Add(readyToJumpEntity);
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