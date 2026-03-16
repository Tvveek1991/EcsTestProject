using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;

namespace Project.Scripts.Gameplay.Systems
{
    public class EndGameSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_endGameFilter;
        
        private EcsPool<InputComponent> m_inputPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            m_endGameFilter = m_world.Filter<EndGame>().End(1);
            
            m_inputPool = m_world.GetPool<InputComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var inputEntity in m_inputFilter)
            {
                if (!m_inputPool.Get(inputEntity).IsEnabled || m_endGameFilter.GetEntitiesCount() == 0)
                    return;
                
                m_inputPool.Get(inputEntity).IsEnabled = false;
            }
        }
    }
}