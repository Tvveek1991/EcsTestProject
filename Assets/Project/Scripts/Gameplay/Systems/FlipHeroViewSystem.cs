using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;

namespace Project.Scripts.Gameplay.Systems
{
    public class FlipHeroViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld m_world;

        private EcsFilter m_inputFilter;
        private EcsFilter m_flipViewFilter;

        private EcsPool<InputComponent> m_inputPool;
        private EcsPool<SpriteRendererComponent> m_spriteRendererPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_inputFilter = m_world.Filter<InputComponent>().End();
            m_flipViewFilter = m_world.Filter<FlipViewComponent>().Inc<SpriteRendererComponent>().End();

            m_inputPool = m_world.GetPool<InputComponent>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var input in m_inputFilter)
            foreach (var flipView in m_flipViewFilter)
            {
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
    }
}