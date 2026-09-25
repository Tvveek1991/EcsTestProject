using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.EntityViewRegistry;
using Project.Scripts.Gameplay.Views;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckHitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IEntityViewRegistry m_entityViewRegistry;

        private const float MAX_DISTANCE = 1.3f;
        private const string LAYER_NAME = "InteractiveObject";

        private EcsWorld m_world;

        private EcsFilter m_hitFilter;
        private EcsFilter m_attackedPersonFilter;

        private EcsPool<Attack> m_attackPool;
        private EcsPool<Health> m_healthPool;
        private EcsPool<HitCommand> m_hitCommandPool;
        private EcsPool<SpriteRendererKeeper> m_spriteRendererPool;

        public CheckHitSystem(IEntityViewRegistry entityViewRegistry)
        {
            m_entityViewRegistry = entityViewRegistry;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitFilter = m_world.Filter<ObjectViewComponent>().Inc<Health>()
                .Exc<HitCommand>().End();
            m_attackedPersonFilter = m_world.Filter<PersonViewComponent>().Inc<Attack>().Inc<SpriteRendererKeeper>().End();

            m_attackPool = m_world.GetPool<Attack>();
            m_healthPool = m_world.GetPool<Health>();
            m_hitCommandPool = m_world.GetPool<HitCommand>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererKeeper>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckAttackedObject();
        }

        private void CheckAttackedObject()
        {
            foreach (var entity in m_attackedPersonFilter)
            {
                if(!m_attackPool.Get(entity).IsActive)
                    continue;
                
                if (!m_entityViewRegistry.TryGet(entity, out PersonView personView))
                    continue;
                
                var checkerTr = personView.GetCheckerSpawnPoint();
                var direction = m_spriteRendererPool.Get(entity).SpriteRenderer.flipX ? Vector3.left : Vector3.right;

                int layerMask = LayerMask.GetMask(LAYER_NAME);
                RaycastHit2D hit = Physics2D.Raycast(checkerTr.position, direction, MAX_DISTANCE, layerMask);
                
                // Debug.DrawLine(checkerTr.position, checkerTr.position + direction * maxDistance, Color.green);
                    
                if (hit.collider != null)
                {
                    foreach (var hitObject in m_hitFilter)
                    {
                        if(!m_entityViewRegistry.TryGet(hitObject, out ObjectView view))
                            continue;
                        
                        if (view.gameObject == hit.collider.gameObject)
                        {
                            if(m_healthPool.Get(hitObject).Count <= 0)
                                return;
                            
                            m_hitCommandPool.Add(hitObject).HitValue = 10;
                            
                            // Debug.Log("Объект обнаружен: " + hit.collider.gameObject.name);
                            
                            break;
                        }
                    }
                }
            }
        }
    }
}
