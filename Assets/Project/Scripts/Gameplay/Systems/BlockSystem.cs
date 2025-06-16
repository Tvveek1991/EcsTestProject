using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class BlockSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_blockFilter;
        private EcsFilter m_withoutBlockFilter;

        private EcsPool<BlockComponent> m_blockPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_blockFilter = m_world.Filter<BlockComponent>().End();
            m_withoutBlockFilter = m_world.Filter<PersonComponent>().Inc<GroundCheckComponent>().Exc<BlockComponent>().Exc<RollingComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_blockPool = m_world.GetPool<BlockComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            AttachBlockComponent();
            DeleteComponent();
        }

        private void AttachBlockComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var personIndex in m_withoutBlockFilter)
            {
                if (m_inputPool.Get(input).IsBlock && m_groundCheckPool.Get(personIndex).GroundSensor.IsConnected)
                    m_blockPool.Add(personIndex).IsAnimate = true;
            }
        }

        private void DeleteComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var blockIndex in m_blockFilter)
            {
                if (!m_inputPool.Get(input).IsBlock)
                    m_blockPool.Del(blockIndex);
            }
        }
    }
}