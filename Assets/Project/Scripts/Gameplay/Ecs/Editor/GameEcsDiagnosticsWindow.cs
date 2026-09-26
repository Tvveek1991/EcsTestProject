using Project.Scripts.Gameplay.Ecs.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Gameplay.Ecs.Editor
{
    public sealed class GameEcsDiagnosticsWindow : EditorWindow
    {
        [MenuItem("Tools/BestWood/ECS Diagnostics")]
        private static void Open() =>
            GetWindow<GameEcsDiagnosticsWindow>("ECS Diagnostics");

        private void Update() =>
            Repaint();

        private void OnGUI()
        {
            GameEcsDiagnosticsSnapshot snapshot = GameEcsDiagnosticsRegistry.ActiveSnapshot;

            if (snapshot == null || !snapshot.IsSessionRunning)
            {
                EditorGUILayout.HelpBox("Start Play Mode to inspect the active ECS session.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Session", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Frame", snapshot.FrameIndex.ToString());
            EditorGUILayout.LabelField("Active phase", snapshot.ActivePhase.ToString());
            EditorGUILayout.LabelField("Active system", snapshot.ActiveSystemName ?? "—");
            EditorGUILayout.LabelField("Presentation operations", snapshot.ActivePresentationOperationCount.ToString());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Key entities", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Player", snapshot.PlayerCount.ToString());
            EditorGUILayout.LabelField("Health", snapshot.HealthCount.ToString());
            EditorGUILayout.LabelField("Coin view", snapshot.CoinViewCount.ToString());
            EditorGUILayout.LabelField("Dead command", snapshot.DeadCommandCount.ToString());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("One-frame commands before cleanup", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Hit", snapshot.OneFrameHitCommandCount.ToString());
            EditorGUILayout.LabelField("Heal", snapshot.OneFrameHealCommandCount.ToString());
            EditorGUILayout.LabelField("Jump", snapshot.OneFrameJumpCommandCount.ToString());
            EditorGUILayout.LabelField("Reaction", snapshot.OneFrameReactionCount.ToString());
            EditorGUILayout.LabelField("Coins counter change", snapshot.OneFrameCoinsCounterChangeCount.ToString());
        }
    }
}
