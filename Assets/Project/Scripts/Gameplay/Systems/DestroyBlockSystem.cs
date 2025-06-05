using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class DestroyBlockSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_blockFilter;
        
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<BlockComponent> m_blockPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_blockFilter = m_world.Filter<BlockComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_blockPool = m_world.GetPool<BlockComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var input in m_inputFilter)
            foreach (var blockIndex in m_blockFilter)
            {
                if(!m_inputPool.Get(input).IsBlock)
                    m_blockPool.Del(blockIndex);
            }
        }
    }
}