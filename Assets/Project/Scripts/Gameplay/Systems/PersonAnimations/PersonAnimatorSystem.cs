using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems.PersonAnimations
{
    public class PersonAnimatorSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly int m_airSpeedY = Animator.StringToHash("AirSpeedY");

        private EcsWorld m_world;

        private EcsFilter m_airSpeedYFilter;

        private EcsPool<AnimatorKeeper> m_animatorPool;
        private EcsPool<Rigidbody2d> m_rigidbody2dPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            SetFilters();
            SetPools();
        }

        private void SetFilters()
        {
            m_airSpeedYFilter = m_world.Filter<AnimatorKeeper>().Inc<Rigidbody2d>().End();
        }

        private void SetPools()
        {
            m_animatorPool = m_world.GetPool<AnimatorKeeper>();
            m_rigidbody2dPool = m_world.GetPool<Rigidbody2d>();
        }

        public void Run(IEcsSystems systems)
        {
            RefreshAirSpeedY();
        }

        private void RefreshAirSpeedY()
        {
            foreach (var entity in m_airSpeedYFilter)
            {
                m_animatorPool.Get(entity).AnimatorController.SetFloat(m_airSpeedY,
                    m_rigidbody2dPool.Get(entity).Rigidbody.linearVelocity.y);
            }
        }

    }
}