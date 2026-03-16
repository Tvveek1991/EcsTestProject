using Gameplay.Services.ObjectsService;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using Project.Scripts.Gameplay.Services.PersonService;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckAttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IPersonViewService m_personViewService;
        private readonly IObjectsService m_objectsService;

        private const float MAX_DISTANCE = 1.3f;
        private const string LAYER_NAME = "InteractiveObject";

        private EcsWorld m_world;

        private EcsFilter m_hitFilter;
        private EcsFilter m_attackedPersonFilter;

        private EcsPool<Health> m_healthPool;
        private EcsPool<HurtCommand> m_hurtCommandPool;
        private EcsPool<AttackCheckComponent> m_attackCheckPool;
        private EcsPool<SpriteRendererKeeper> m_spriteRendererPool;

        public CheckAttackSystem(IPersonViewService personViewService, IObjectsService objectsService)
        {
            m_objectsService = objectsService;
            m_personViewService = personViewService;
        }
        
        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_hitFilter = m_world.Filter<ObjectViewComponent>().Inc<Health>()
                .Exc<HurtCommand>().End();
            m_attackedPersonFilter = m_world.Filter<PersonViewComponent>().Inc<AttackCheckComponent>().Inc<SpriteRendererKeeper>().End();
            
            m_healthPool = m_world.GetPool<Health>();
            m_hurtCommandPool = m_world.GetPool<HurtCommand>();
            m_attackCheckPool = m_world.GetPool<AttackCheckComponent>();
            m_spriteRendererPool = m_world.GetPool<SpriteRendererKeeper>();
        }

        public void Run(IEcsSystems systems)
        {
            CheckAttackedObject();
        }

        private void CheckAttackedObject()
        {
            foreach (var person in m_attackedPersonFilter)
            {
                m_attackCheckPool.Del(person);
                
                var personView = m_personViewService.GetPersonViewByEntity(person);
                
                if(personView == null)
                    continue;
                
                var checkerTr = personView.GetCheckerSpawnPoint();
                var direction = m_spriteRendererPool.Get(person).SpriteRenderer.flipX ? Vector3.left : Vector3.right;

                int layerMask = LayerMask.GetMask(LAYER_NAME);
                RaycastHit2D hit = Physics2D.Raycast(checkerTr.position, direction, MAX_DISTANCE, layerMask);
                
                // Debug.DrawLine(checkerTr.position, checkerTr.position + direction * maxDistance, Color.green);
                    
                if (hit.collider != null)
                {
                    foreach (var hitObject in m_hitFilter)
                    {
                        if(!m_objectsService.Views.TryGetValue(hitObject, out var view))
                            continue;
                        
                        if (view.gameObject == hit.collider.gameObject)
                        {
                            if(m_healthPool.Get(hitObject).Count <= 0)
                                return;
                            
                            m_hurtCommandPool.Add(hitObject).HitValue = 10;
                            
                            // Debug.Log("Объект обнаружен: " + hit.collider.gameObject.name);
                            
                            break;
                        }
                    }
                }
            }
        }
    }
}