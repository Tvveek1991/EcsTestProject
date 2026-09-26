using System;
using System.Reflection;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.Input
{
    public sealed class GameplayInputReader : IGameplayInputReader, IDisposable
    {
        private const string InputActionsResourcePath = "Input/GameplayInputActions";
        private const string InputActionAssetTypeName = "UnityEngine.InputSystem.InputActionAsset, Unity.InputSystem";

        private readonly object m_gameplayActionMap;
        private readonly object m_moveAction;
        private readonly object m_jumpAction;
        private readonly object m_rollAction;
        private readonly object m_attackAction;
        private readonly object m_blockAction;
        private readonly object m_debugHurtAction;
        private readonly object m_debugDeathAction;
        private readonly MethodInfo m_readValueMethod;
        private readonly MethodInfo m_wasPressedThisFrameMethod;
        private readonly MethodInfo m_isPressedMethod;

        public GameplayInputSnapshot Snapshot { get; private set; }

        public GameplayInputReader()
        {
            var inputActionAssetType = Type.GetType(InputActionAssetTypeName);
            if (inputActionAssetType == null)
                throw new InvalidOperationException("The Unity Input System package is not available at runtime.");

            var inputActions = Resources.Load(InputActionsResourcePath, inputActionAssetType);
            if (inputActions == null)
                throw new InvalidOperationException($"Input action asset was not found at Resources/{InputActionsResourcePath}.");

            m_gameplayActionMap = FindActionMap(inputActions, "Gameplay");
            m_moveAction = FindAction("Move");
            m_jumpAction = FindAction("Jump");
            m_rollAction = FindAction("Roll");
            m_attackAction = FindAction("Attack");
            m_blockAction = FindAction("Block");
            m_debugHurtAction = FindAction("DebugHurt");
            m_debugDeathAction = FindAction("DebugDeath");

            var actionType = m_moveAction.GetType();
            m_readValueMethod = actionType.GetMethod("ReadValue");
            m_wasPressedThisFrameMethod = actionType.GetMethod("WasPressedThisFrame");
            m_isPressedMethod = actionType.GetMethod("IsPressed");

            m_gameplayActionMap.GetType().GetMethod("Enable").Invoke(m_gameplayActionMap, null);
        }

        public void UpdateSnapshot()
        {
            float move = (float)m_readValueMethod.MakeGenericMethod(typeof(float)).Invoke(m_moveAction, null);

            Snapshot = new GameplayInputSnapshot
            {
                IsEnabled = true,
                IsJump = WasPressedThisFrame(m_jumpAction),
                IsRolling = WasPressedThisFrame(m_rollAction),
                IsMoveLeft = move < 0,
                IsMoveRight = move > 0,
                IsHurt = WasPressedThisFrame(m_debugHurtAction),
                IsDead = WasPressedThisFrame(m_debugDeathAction),
                IsAttack = WasPressedThisFrame(m_attackAction),
                IsBlock = (bool)m_isPressedMethod.Invoke(m_blockAction, null)
            };
        }

        public void Dispose()
        {
            m_gameplayActionMap.GetType().GetMethod("Disable").Invoke(m_gameplayActionMap, null);
        }

        private static object FindActionMap(object inputActions, string actionMapName)
        {
            return inputActions.GetType().GetMethod("FindActionMap", new[] { typeof(string), typeof(bool) })
                .Invoke(inputActions, new object[] { actionMapName, true });
        }

        private object FindAction(string actionName)
        {
            return m_gameplayActionMap.GetType().GetMethod("FindAction", new[] { typeof(string), typeof(bool) })
                .Invoke(m_gameplayActionMap, new object[] { actionName, true });
        }

        private bool WasPressedThisFrame(object action) =>
            (bool)m_wasPressedThisFrameMethod.Invoke(action, null);
    }
}
