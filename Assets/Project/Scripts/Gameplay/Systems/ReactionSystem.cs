using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.ReactionService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class ReactionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IReactionService m_reactionService;
        private EcsWorld m_world;
        
        private EcsFilter m_reactionFilter;
        private EcsPool<ReactionComponent> m_reactionPool;

        public ReactionSystem(IReactionService reactionService)
        {
            m_reactionService = reactionService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();
            
            m_reactionFilter = m_world.Filter<ReactionComponent>().End();
            
            m_reactionPool = m_world.GetPool<ReactionComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var reaction in m_reactionFilter)
            {
                var reactionComponent = m_reactionPool.Get(reaction);

                switch (reactionComponent.Type)
                {
                    case ReactionType.CompleteState:
                        m_reactionPool.Del(reaction);
                        m_reactionService.RestartGame();
                        return;
                    default:
                        break;
                }

                // очищаем реакцию, чтобы не обрабатывать повторно
                m_reactionPool.Del(reaction);
            }
        }
    }
}
