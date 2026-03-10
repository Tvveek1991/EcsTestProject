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

        private EcsPool<Run> m_runPool;
        private EcsPool<WallCheck> m_wallCheckPool;
        private EcsPool<GroundCheckComponent> m_groundCheckPool;
        private EcsPool<SpriteRendererKeeper> m_spriteRendererPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_noRunFilter = m_world.Filter<SpriteRendererKeeper>()
                .Exc<PlayableObject>().Exc<Run>().Exc<Rolling>().Exc<Block>().End();
            m_runFilter = m_world.Filter<Run>().Inc<SpriteRendererKeeper>()
                .Exc<Rolling>().Exc<Block>().End();
            m_wallCheckFilter = m_world.Filter<AnimatorKeeper>().Inc<WallCheck>().End();
            m_groundCheckFilter = m_world.Filter<AnimatorKeeper>().Inc<GroundCheckComponent>().End();

            m_runPool = m_world.GetPool<Run>();
            m_wallCheckPool = m_world.GetPool<WallCheck>();
            m_groundCheckPool = m_world.GetPool<GroundCheckComponent>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererKeeper>();
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
                m_groundCheckPool.Get(groundIndex).GroundSensors.Any(item => item.IsConnected))
                return;

            var connectedSensor = m_wallCheckPool.Get(wallIndex).WallSensors.FirstOrDefault(item => item.IsConnected);

            if (connectedSensor != null)
                m_spriteRendererPool.Get(flipView).SpriteRenderer.flipX = connectedSensor.transform.localPosition.x < 0;
        }
    }
}