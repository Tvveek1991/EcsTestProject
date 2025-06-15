using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class FlipHeroViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_flipViewFilter;
        private EcsFilter m_wallCheckFilter;
        private EcsFilter m_groundCheckFilter;

        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<WallCheckComponent> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<SpriteRendererComponent> m_spriteRendererPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_flipViewFilter = m_world.Filter<FlipViewComponent>().Inc<SpriteRendererComponent>().End();
            m_wallCheckFilter = m_world.Filter<AnimatorComponent>().Inc<WallCheckComponent>().End();
            m_groundCheckFilter = m_world.Filter<AnimatorComponent>().Inc<GroundCheckComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_wallCheckPool = m_world.GetPool<WallCheckComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var flipView in m_flipViewFilter)
            foreach (var input in m_inputFilter)
            foreach (var wallIndex in m_wallCheckFilter)
            foreach (var groundIndex in m_groundCheckFilter)
            {
                if (CheckSideInSlide(wallIndex, groundIndex, flipView))
                    continue;

                if (m_inputPool.Get(input).IsMoveRightPressed)
                {
                    m_spriteRendererPool.Get(flipView).SpriteRenderer.flipX = false;
                }
                else if (m_inputPool.Get(input).IsMoveLeftPressed)
                {
                    m_spriteRendererPool.Get(flipView).SpriteRenderer.flipX = true;
                }
            }
        }

        private bool CheckSideInSlide(int wallIndex, int groundIndex, int flipView)
        {
            if (m_wallCheckPool.Get(wallIndex).WallSensors == null)
                return false;

            if (!m_wallCheckPool.Get(wallIndex).WallSensors.Any(item => item.IsConnected) ||
                m_groundCheckPool.Get(groundIndex).GroundSensor.IsConnected)
                return false;

            var connectedSensor = m_wallCheckPool.Get(wallIndex).WallSensors.FirstOrDefault(item => item.IsConnected);

            if (connectedSensor != null)
                m_spriteRendererPool.Get(flipView).SpriteRenderer.flipX = connectedSensor.transform.localPosition.x < 0;

            return true;
        }
    }
}