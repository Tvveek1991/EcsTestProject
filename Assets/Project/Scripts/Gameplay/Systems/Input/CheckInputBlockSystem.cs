using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public class CheckInputBlockSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_blockFilter;
        private EcsFilter m_readyToBlockFilter;

        private EcsPool<Block> m_blockPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            m_blockFilter = m_world.Filter<Block>().End();
            m_readyToBlockFilter = m_world.Filter<Player>().Inc<GroundCheckComponent>()
                .Exc<Block>().Exc<Rolling>().End(1);

            m_blockPool = m_world.GetPool<Block>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!CheckInput())
                return;
            
            AttachBlockComponent();
            CheckUnblock();
        }

        private void AttachBlockComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var readyToBlockEntity in m_readyToBlockFilter)
            {
                if (m_inputPool.Get(input).IsBlock && m_groundCheckPool.Get(readyToBlockEntity).GroundSensors.Any(item => item.IsConnected))
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
        
        private bool CheckInput()
        {
            foreach (var inputEntity in m_inputFilter)
                return m_inputPool.Get(inputEntity).IsEnabled;

            return true;
        }
    }
}