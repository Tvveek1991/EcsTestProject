using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class BlockSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_withoutBlockFilter;

        private EcsPool<BlockComponent> m_blockPool;
        private EcsPool<InputComponent> m_inputPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_withoutBlockFilter = m_world.Filter<PersonComponent>().Exc<BlockComponent>().Exc<RollingComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_blockPool = m_world.GetPool<BlockComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var input in m_inputFilter)
            foreach (var personIndex in m_withoutBlockFilter)
            {
                if (m_inputPool.Get(input).IsBlock) 
                    m_blockPool.Add(personIndex);
            }
        }
    }
}