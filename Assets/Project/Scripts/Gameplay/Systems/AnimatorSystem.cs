using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class AnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_jump = Animator.StringToHash("Jump");
        private readonly int m_grounded = Animator.StringToHash("Grounded");
        private readonly int m_animState = Animator.StringToHash("AnimState");
        private readonly int m_airSpeedY = Animator.StringToHash("AirSpeedY");
        private readonly int m_death = Animator.StringToHash("Death");
        private readonly int m_hurt = Animator.StringToHash("Hurt");

        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_heroFilter;
        private EcsFilter m_runFilter;
        private EcsFilter m_jumperFilter;
        private EcsFilter m_airSpeedYFilter;
        private EcsFilter m_groundCheckFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<JumpComponent> m_jumpPool;
        private EcsPool<AnimatorComponent> m_animatorPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_heroFilter = m_world.Filter<AnimatorComponent>().Inc<HeroComponent>().End();
            m_runFilter = m_world.Filter<AnimatorComponent>().Inc<RunComponent>().End();
            m_jumperFilter = m_world.Filter<AnimatorComponent>().Inc<JumpComponent>().End();
            m_airSpeedYFilter = m_world.Filter<AnimatorComponent>().Inc<Rigidbody2dComponent>().End();
            m_groundCheckFilter = m_world.Filter<AnimatorComponent>().Inc<GroundCheckComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_runPool = m_world.GetPool<RunComponent>();
            m_animatorPool = m_world.GetPool<AnimatorComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            RefreshAirSpeedY();
            CheckFalling();
            CheckRun();
            CheckJump();
            CheckSimpleAnimation();
        }

        private void CheckSimpleAnimation()
        {
            foreach (var input in m_inputFilter)
            foreach (var index in m_heroFilter)
            {
                if (m_inputPool.Get(input).IsDead)
                    m_animatorPool.Get(index).AnimatorController.SetTrigger(m_death);
                else if(m_inputPool.Get(input).IsHurt)
                    m_animatorPool.Get(index).AnimatorController.SetTrigger(m_hurt);
            }
        }

        private void RefreshAirSpeedY()
        {
            foreach (var index in m_airSpeedYFilter)
            {
                m_animatorPool.Get(index).AnimatorController.SetFloat(m_airSpeedY,
                    m_rigidbody2dPool.Get(index).Rigidbody.linearVelocity.y);
            }
        }

        private void CheckFalling()
        {
            foreach (var index in m_groundCheckFilter)
            {
                if (m_groundCheckPool.Get(index).GroundSensor.IsGrounded)
                {
                    m_animatorPool.Get(index).AnimatorController.SetBool(m_grounded, true);
                }

                if (!m_groundCheckPool.Get(index).GroundSensor.IsGrounded)
                {
                    m_animatorPool.Get(index).AnimatorController.SetBool(m_grounded, false);
                }
            }
        }

        private void CheckRun()
        {
            foreach (var index in m_runFilter)
            {
                m_animatorPool.Get(index).AnimatorController
                    .SetInteger(m_animState, m_runPool.Get(index).IsRun ? 1 : 0);
            }
        }

        private void CheckJump()
        {
            foreach (var jumper in m_jumperFilter)
            {
                m_animatorPool.Get(jumper).AnimatorController.SetTrigger(m_jump);
                m_animatorPool.Get(jumper).AnimatorController.SetBool(m_grounded, false);
            }
        }
    }
}