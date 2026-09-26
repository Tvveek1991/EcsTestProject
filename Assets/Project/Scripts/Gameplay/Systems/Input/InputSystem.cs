using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components.Input;
using Project.Scripts.Gameplay.Services.Input;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public sealed class InputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IGameplayInputReader m_gameplayInputReader;

        private EcsWorld m_world;
    
        private EcsFilter m_inputFilter;
        
        private EcsPool<InputComponent> m_inputPool;

        public InputSystem(IGameplayInputReader gameplayInputReader)
        {
            m_gameplayInputReader = gameplayInputReader;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End(1);
            
            m_inputPool = m_world.GetPool<InputComponent>();

            CreateInputComponent();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in m_inputFilter)
            {
                ref var input = ref m_inputPool.Get(i);
                GameplayInputSnapshot snapshot = m_gameplayInputReader.Snapshot;

                input.IsEnabled = snapshot.IsEnabled;
                input.IsJump = snapshot.IsJump;
                input.IsRolling = snapshot.IsRolling;
                input.IsMoveLeft = snapshot.IsMoveLeft;
                input.IsMoveRight = snapshot.IsMoveRight;
                input.IsDead = snapshot.IsDead;
                input.IsHurt = snapshot.IsHurt;
                input.IsAttack = snapshot.IsAttack;
                input.IsBlock = snapshot.IsBlock;
            }
        }

        private void CreateInputComponent()
        {
            var entity = m_world.NewEntity();
            m_inputPool.Add(entity).IsEnabled = true;
        }
    }
}
