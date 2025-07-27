using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class DisableInputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_deadFilter;
        private EcsFilter m_inputFilter;
        
        private EcsPool<InputComponent> m_inputPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_deadFilter = m_world.Filter<PlayerComponent>().Inc<DeadCommandComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var input in m_inputFilter)
            {
                if(m_inputPool.Get(input).IsEnabled && m_deadFilter.GetEntitiesCount() > 0)
                    m_inputPool.Get(input).IsEnabled = false;
            }
        }
    }
}