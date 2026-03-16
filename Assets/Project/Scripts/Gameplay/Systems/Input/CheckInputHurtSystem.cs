using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Components.Input;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public class CheckInputHurtSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_hitFilter;
        private EcsFilter m_inputFilter;

        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<HitCommand> m_hitCommandPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            m_hitFilter = m_world.Filter<Player>().Inc<Health>()
                .Exc<HitCommand>().End(1);

            m_inputPool = m_world.GetPool<InputComponent>();
            m_hitCommandPool = m_world.GetPool<HitCommand>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!CheckInput())
                return;
            
            AttachHurtComponent();
        }

        private void AttachHurtComponent()
        {
            foreach (var input in m_inputFilter)
            foreach (var hitEntity in m_hitFilter)
            {
                if (m_inputPool.Get(input).IsHurt) 
                    m_hitCommandPool.Add(hitEntity).HitValue = 25;
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