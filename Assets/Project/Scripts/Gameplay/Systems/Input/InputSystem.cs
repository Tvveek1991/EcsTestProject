using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components.Input;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.Input
{
    public sealed class InputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
    
        private EcsFilter m_inputFilter;
        
        private EcsPool<InputComponent> m_inputPool;
        
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
            
                input.IsJumpPressed = UnityEngine.Input.GetKeyDown(KeyCode.Space);
                input.IsRollPressed = UnityEngine.Input.GetKeyDown(KeyCode.LeftShift);
                
                input.IsMoveLeftPressed = UnityEngine.Input.GetKey(KeyCode.A);
                input.IsMoveRightPressed = UnityEngine.Input.GetKey(KeyCode.D);
                
                input.IsDead = UnityEngine.Input.GetKeyDown(KeyCode.E);
                input.IsHurt = UnityEngine.Input.GetKeyDown(KeyCode.Q);
                
                input.IsAttack = UnityEngine.Input.GetKeyDown(KeyCode.Mouse0);
                input.IsBlock = UnityEngine.Input.GetKey(KeyCode.Mouse1);
            }
        }

        private void CreateInputComponent()
        {
            var entity = m_world.NewEntity();
            m_inputPool.Add(entity).IsEnabled = true;
        }
    }
}
