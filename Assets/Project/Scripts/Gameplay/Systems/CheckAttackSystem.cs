using Leopotam.EcsLite;
using Project.Scripts.Gameplay.Components;
using UnityEngine;

namespace Project.Scripts.Gameplay.Systems
{
    public class CheckAttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float MAX_DISTANCE = 1.3f;
        private const string LAYER_NAME = "InteractiveObject";

        private EcsWorld m_world;

        private EcsFilter m_hitFilter;
        private EcsFilter m_attackedPersonFilter;
        // private EcsFilter m_attackCheckFilter;

        private EcsPool<HurtCommandComponent> m_hurtCommandPool;
        private EcsPool<AttackCheckComponent> m_attackCheckPool;
        private EcsPool<ObjectViewRefComponent> m_objectViewRefPool;
        private EcsPool<SpriteRendererComponent> m_spriteRendererPool;
        private EcsPool<PersonViewRefComponent> m_personViewRefComponentPool;

        public void Init(IEcsSystems systems)
        {
            m_world = systems.GetWorld();

            m_attackedPersonFilter = m_world.Filter<PersonViewRefComponent>().Inc<AttackCheckComponent>().Inc<SpriteRendererComponent>().End();
            m_hitFilter = m_world.Filter<ObjectViewRefComponent>().Inc<HealthComponent>()
                .Exc<HurtCommandComponent>().End();

            m_attackCheckPool = m_world.GetPool<AttackCheckComponent>();
            
            m_hurtCommandPool = m_world.GetPool<HurtCommandComponent>();
            m_objectViewRefPool = m_world.GetPool<ObjectViewRefComponent>();
            
            m_spriteRendererPool = m_world.GetPool<SpriteRendererComponent>();
            m_personViewRefComponentPool = m_world.GetPool<PersonViewRefComponent>();
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
                
                var checkerTr = m_personViewRefComponentPool.Get(person).PersonView.GetCheckerSpawnPoint();
                var direction = m_spriteRendererPool.Get(person).SpriteRenderer.flipX ? Vector3.left : Vector3.right;

                int layerMask = LayerMask.GetMask(LAYER_NAME);
                RaycastHit2D hit = Physics2D.Raycast(checkerTr.position, direction, MAX_DISTANCE, layerMask);
                
                // Debug.DrawLine(checkerTr.position, checkerTr.position + direction * maxDistance, Color.green);
                    
                if (hit.collider != null)
                {
                    foreach (var hitObject in m_hitFilter)
                    {
                        if (m_objectViewRefPool.Get(hitObject).ObjectView.gameObject == hit.collider.gameObject)
                        {
                            m_hurtCommandPool.Add(hitObject).HitValue = 10;
                            
                            Debug.Log("Объект обнаружен: " + hit.collider.gameObject.name);
                            
                            break;
                        }
                    }
                }
            }
        }
    }
}