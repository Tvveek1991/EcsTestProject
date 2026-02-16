using System.Linq;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class FlipHeroViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_runFilter;
        private EcsFilter m_noRunFilter;
        private EcsFilter m_wallCheckFilter;
        private EcsFilter m_groundCheckFilter;

        private EcsPool<RunComponent> m_runPool;
        private EcsPool<WallCheckComponent> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<SpriteRendererComponent> m_spriteRendererPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_noRunFilter = m_world.Filter<SpriteRendererComponent>()
                .Exc<RunComponent>().Exc<RollingComponent>().Exc<BlockComponent>().Exc<DeadComponent>().End();
            m_runFilter = m_world.Filter<RunComponent>().Inc<SpriteRendererComponent>()
                .Exc<RollingComponent>().Exc<BlockComponent>().Exc<DeadComponent>().End();
            m_wallCheckFilter = m_world.Filter<AnimatorComponent>().Inc<WallCheckComponent>().End();
            m_groundCheckFilter = m_world.Filter<AnimatorComponent>().Inc<GroundCheckComponent>().End();

            m_runPool = m_world.GetPool<RunComponent>();
            m_wallCheckPool = m_world.GetPool<WallCheckComponent>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var noRunEntity in m_noRunFilter)
            foreach (var wallIndex in m_wallCheckFilter)
            foreach (var groundIndex in m_groundCheckFilter)
            {
                CheckSideInSlide(wallIndex, groundIndex, noRunEntity);
            }

            foreach (var runEntity in m_runFilter)
            foreach (var wallIndex in m_wallCheckFilter)
            {
                if (m_wallCheckPool.Get(wallIndex).WallSensors == null)
                    return;
                
                if (m_wallCheckPool.Get(wallIndex).WallSensors.Any(item => item.IsConnected))
                    return;
                
                var flipDirection = m_runPool.Get(runEntity).Direction;
                if(flipDirection == 0)
                    continue;
                
                m_spriteRendererPool.Get(runEntity).SpriteRenderer.flipX = flipDirection < 0;
            }
        }

        private void CheckSideInSlide(int wallIndex, int groundIndex, int flipView)
        {
            if (m_wallCheckPool.Get(wallIndex).WallSensors == null)
                return;

            if (!m_wallCheckPool.Get(wallIndex).WallSensors.Any(item => item.IsConnected) ||
                m_groundCheckPool.Get(groundIndex).GroundSensor.IsConnected)
                return;

            var connectedSensor = m_wallCheckPool.Get(wallIndex).WallSensors.FirstOrDefault(item => item.IsConnected);

            if (connectedSensor != null)
                m_spriteRendererPool.Get(flipView).SpriteRenderer.flipX = connectedSensor.transform.localPosition.x < 0;
        }
    }
}