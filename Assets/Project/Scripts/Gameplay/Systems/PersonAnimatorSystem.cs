using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class PersonAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const string ATTACK_KEY = "Attack";

        private readonly int m_jump = Animator.StringToHash("Jump");
        private readonly int m_wallSliding = Animator.StringToHash("WallSlide");
        private readonly int m_grounded = Animator.StringToHash("Grounded");
        private readonly int m_animState = Animator.StringToHash("AnimState");
        private readonly int m_airSpeedY = Animator.StringToHash("AirSpeedY");
        private readonly int m_death = Animator.StringToHash("Death");
        private readonly int m_hurt = Animator.StringToHash("Hurt");
        private readonly int m_roll = Animator.StringToHash("Roll");
        private readonly int m_block = Animator.StringToHash("Block");
        private readonly int m_blocked = Animator.StringToHash("Blocked");
        private readonly int m_idleBlock = Animator.StringToHash("IdleBlock");

        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_heroFilter;
        private EcsFilter m_blockFilter;
        private EcsFilter m_attackFilter;
        private EcsFilter m_runFilter;
        private EcsFilter m_outOfRunFilter;
        private EcsFilter m_jumperFilter;
        private EcsFilter m_rollingFilter;
        private EcsFilter m_airSpeedYFilter;
        private EcsFilter m_wallCheckFilter;
        private EcsFilter m_groundCheckFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<JumpComponent> m_jumpPool;
        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<AttackComponent> m_attackPool;
        private EcsPool<RollingComponent> m_rollingPool;
        private EcsPool<AnimatorComponent> m_animatorPool;
        private EcsPool<Rigidbody2dComponent> m_rigidbody2dPool;
        private EcsPool<WallCheckComponent> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_runFilter = m_world.Filter<AnimatorComponent>().Inc<RunComponent>().End();
            m_outOfRunFilter = m_world.Filter<AnimatorComponent>().Exc<RunComponent>().End();
            m_heroFilter = m_world.Filter<AnimatorComponent>().Inc<HeroComponent>().End();
            m_jumperFilter = m_world.Filter<AnimatorComponent>().Inc<JumpComponent>().End();
            m_rollingFilter = m_world.Filter<AnimatorComponent>().Inc<RollingComponent>().End();
            m_airSpeedYFilter = m_world.Filter<AnimatorComponent>().Inc<Rigidbody2dComponent>().End();
            m_wallCheckFilter = m_world.Filter<AnimatorComponent>().Inc<WallCheckComponent>().End();
            m_groundCheckFilter = m_world.Filter<AnimatorComponent>().Inc<GroundCheckComponent>().End();
            m_attackFilter = m_world.Filter<AnimatorComponent>().Inc<HeroComponent>().Exc<RollingComponent>().End();
            m_blockFilter = m_world.Filter<AnimatorComponent>().Inc<HeroComponent>().Inc<BlockComponent>().End();

            m_runPool = m_world.GetPool<RunComponent>();
            m_inputPool = m_world.GetPool<InputComponent>();
            m_attackPool = m_world.GetPool<AttackComponent>();
            m_rollingPool = m_world.GetPool<RollingComponent>();
            m_animatorPool = m_world.GetPool<AnimatorComponent>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2dComponent>();
            m_wallCheckPool = m_world.GetPool<WallCheckComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckRun();
            RefreshAirSpeedY();
            CheckFalling();
            CheckJump();
            CheckRolling();
            CheckBlock();
            CheckAttack();
            CheckSliding();
            CheckSimpleAnimation();
        }

        private void CheckSimpleAnimation()
        {
            foreach (var input in m_inputFilter)
            foreach (var index in m_heroFilter)
            {
                if (m_inputPool.Get(input).IsDead)
                    m_animatorPool.Get(index).AnimatorController.SetTrigger(m_death);
                else if (m_inputPool.Get(input).IsHurt)
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
                if (m_groundCheckPool.Get(index).GroundSensor.IsConnected)
                    m_animatorPool.Get(index).AnimatorController.SetBool(m_grounded, true);

                if (!m_groundCheckPool.Get(index).GroundSensor.IsConnected)
                    m_animatorPool.Get(index).AnimatorController.SetBool(m_grounded, false);
            }
        }

        private void CheckRun()
        {
            foreach (var index in m_runFilter)
                m_animatorPool.Get(index).AnimatorController.SetInteger(m_animState, 1);

            foreach (var index in m_outOfRunFilter)
                m_animatorPool.Get(index).AnimatorController.SetInteger(m_animState, 0);
        }

        private void CheckJump()
        {
            foreach (var jumper in m_jumperFilter)
            {
                m_animatorPool.Get(jumper).AnimatorController.SetTrigger(m_jump);
                m_animatorPool.Get(jumper).AnimatorController.SetBool(m_grounded, false);
            }
        }

        private void CheckRolling()
        {
            foreach (var roller in m_rollingFilter)
            {
                if (!m_rollingPool.Get(roller).IsRolling)
                {
                    m_rollingPool.Get(roller).IsRolling = true;
                    m_animatorPool.Get(roller).AnimatorController.SetTrigger(m_roll);
                }
            }
        }

        private void CheckBlock()
        {
            foreach (var input in m_inputFilter)
            foreach (var personIndex in m_blockFilter)
            {
                if (m_inputPool.Get(input).IsBlock)
                {
                    m_animatorPool.Get(personIndex).AnimatorController.SetTrigger(m_block);
                    m_animatorPool.Get(personIndex).AnimatorController.SetBool(m_idleBlock, true);
                }
                else if (!m_inputPool.Get(input).IsBlock)
                {
                    m_animatorPool.Get(personIndex).AnimatorController.SetBool(m_idleBlock, false);
                }
            }
        }

        private void CheckAttack()
        {
            foreach (var input in m_inputFilter)
            foreach (var personIndex in m_attackFilter)
            {
                if (m_inputPool.Get(input).IsAttack)
                {
                    m_animatorPool.Get(personIndex).AnimatorController
                        .SetTrigger($"{ATTACK_KEY}{m_attackPool.Get(personIndex).CurrentAttackIndex}");
                }
            }
        }

        private void CheckSliding()
        {
            foreach (var wallIndex in m_wallCheckFilter)
            {
                if (m_wallCheckPool.Get(wallIndex).WallSensors != null)
                    m_animatorPool.Get(wallIndex).AnimatorController.SetBool(m_wallSliding, m_wallCheckPool.Get(wallIndex).WallSensors.Any(item => item.IsConnected));
            }
        }
    }
}