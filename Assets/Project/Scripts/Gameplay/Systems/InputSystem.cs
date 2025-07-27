using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public sealed class InputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;
    
        private EcsFilter m_inputFilter;
        
        private EcsPool<InputComponent> m_inputPool;
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_inputPool = m_world.GetPool<InputComponent>();

            CreateInputComponent();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in m_inputFilter)
            {
                ref var input = ref m_inputPool.Get(i);
            
                input.IsJumpPressed = Input.GetKeyDown(KeyCode.Space);
                input.IsRollPressed = Input.GetKeyDown(KeyCode.LeftShift);
                
                input.IsMoveLeftPressed = Input.GetKey(KeyCode.A);
                input.IsMoveRightPressed = Input.GetKey(KeyCode.D);
                
                input.IsDead = Input.GetKeyDown(KeyCode.E);
                input.IsHurt = Input.GetKeyDown(KeyCode.Q);
                
                input.IsAttack = Input.GetKeyDown(KeyCode.Mouse0);
                input.IsBlock = Input.GetKey(KeyCode.Mouse1);
            }
        }

        private void CreateInputComponent()
        {
            var entity = m_world.NewEntity();
            m_inputPool.Add(entity).IsEnabled = true;
        }
    }
}
