using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Project.Scripts.Gameplay;
using Project.Scripts.Gameplay.Ecs.Diagnostics;
using Project.Scripts.Gameplay.Systems;
using Project.Scripts.Gameplay.Systems.Input;
using Project.Scripts.Gameplay.Systems.PersonAnimations;

namespace Project.Scripts.Gameplay.Ecs
{
    public sealed class GameSystemsComposer
    {
        private static readonly Type[] OrderedSystemTypes =
        {
            // Initialization
            typeof(CanvasInitSystem),
            typeof(CreateGameLevelViewSystem),
            typeof(CameraInitSystem),
            typeof(PlayerInitSystem),
            typeof(BoxViewInitSystem),
            typeof(CoinsCounterInitSystem),
            typeof(CoinsCounterViewInitSystem),
            typeof(CoinsViewInitSystem),
            typeof(CreateTutorialViewSystem),
            typeof(PersonConnectSensorsInitSystem),

            // Input
            typeof(InputSystem),
            typeof(CheckInputJumpSystem),
            typeof(CheckInputRollSystem),
            typeof(CheckInputMoveSystem),
            typeof(CheckInputHurtSystem),
            typeof(CheckInputAttackSystem),
            typeof(CheckInputBlockSystem),

            // Simulation
            typeof(ReactionSystem),
            typeof(HealthInitSystem),
            typeof(CoinsCounterChangeSystem),
            typeof(CoinsViewCheckSystem),
            typeof(AttackSystem),
            typeof(CheckHitSystem),
            typeof(HealthChangeSystem),

            // Physics
            typeof(JumpSystem),
            typeof(BlockSystem),
            typeof(RunSystem),
            typeof(RollingSystem),

            // Presentation
            typeof(CameraFollowSystem),
            typeof(CoinsCounterViewChangeSystem),
            typeof(CoinsViewAnimationSystem),
            typeof(HealthViewInitSystem),
            typeof(HealthViewFollowSystem),
            typeof(HealthViewChangeSystem),
            typeof(FlipHeroViewSystem),
            typeof(PersonAnimatorSystem),
            typeof(PersonMoveAnimatorSystem),
            typeof(PersonFallingAnimatorSystem),
            typeof(PersonJumpAnimatorSystem),
            typeof(PersonRollingAnimatorSystem),
            typeof(PersonBlockAnimatorSystem),
            typeof(PersonAttackAnimatorSystem),
            typeof(PersonSlidingAnimatorSystem),
            typeof(PersonHurtAnimatorSystem),
            typeof(PersonDeadAnimatorSystem),
            typeof(CheckDeathSystem),
            typeof(FinishViewInitSystem),
            typeof(EndGameSystem),

            // Cleanup
            typeof(DestroyHealthViewSystem),
            typeof(DestroyObjectViewSystem),
            typeof(EndOfFrameCleanupSystem)
        };

        public static int GetOrderIndex(Type systemType)
        {
            if (systemType == null)
                throw new ArgumentNullException(nameof(systemType));

            for (int index = 0; index < OrderedSystemTypes.Length; index++)
            {
                if (OrderedSystemTypes[index] == systemType)
                    return index;
            }

            return -1;
        }

        public static GameEcsPhase GetPhase(Type systemType)
        {
            int systemIndex = GetOrderIndex(systemType);

            if (systemIndex < 0)
                return GameEcsPhase.None;

            if (systemIndex < GetOrderIndex(typeof(InputSystem)))
                return GameEcsPhase.Initialization;

            if (systemIndex < GetOrderIndex(typeof(ReactionSystem)))
                return GameEcsPhase.Input;

            if (systemIndex < GetOrderIndex(typeof(JumpSystem)))
                return GameEcsPhase.Simulation;

            if (systemIndex < GetOrderIndex(typeof(CameraFollowSystem)))
                return GameEcsPhase.Physics;

            if (systemIndex < GetOrderIndex(typeof(DestroyHealthViewSystem)))
                return GameEcsPhase.Presentation;

            return GameEcsPhase.Cleanup;
        }

        public IReadOnlyList<IEcsSystem> Compose(IEnumerable<IEcsSystem> registeredSystems)
        {
            if (registeredSystems == null)
                throw new ArgumentNullException(nameof(registeredSystems));

            var systemsByType = new Dictionary<Type, IEcsSystem>();

            foreach (IEcsSystem system in registeredSystems)
            {
                if (system == null)
                    throw new InvalidOperationException("ECS system registration cannot contain null.");

                Type systemType = system.GetType();

                if (systemsByType.ContainsKey(systemType))
                    throw new InvalidOperationException($"ECS system {systemType.Name} is registered more than once.");

                systemsByType.Add(systemType, system);
            }

            var orderedSystems = new List<IEcsSystem>(OrderedSystemTypes.Length);

            foreach (Type systemType in OrderedSystemTypes)
            {
                if (!systemsByType.TryGetValue(systemType, out IEcsSystem system))
                    throw new InvalidOperationException($"ECS system {systemType.Name} is missing from the current session.");

                orderedSystems.Add(system);
                systemsByType.Remove(systemType);
            }

            foreach (Type systemType in systemsByType.Keys)
                throw new InvalidOperationException($"ECS system {systemType.Name} is not declared in {nameof(GameSystemsComposer)}.");

            return orderedSystems;
        }
    }
}
