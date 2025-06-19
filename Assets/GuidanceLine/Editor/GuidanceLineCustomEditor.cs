using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace GuidanceLine
{
    /// <summary>
    /// Custom editor for Dynamic GuidanceLine:
    /// - Uses 'player' as dynamic start
    /// - Exposes 'dynamicLength' instead of endPoint
    /// - Simplified interface
    /// </summary>
    [CustomEditor(typeof(GuidanceLine))]
    public class GuidanceLineCustomEditor : Editor
    {
        SerializedProperty toggleGizmos;
        SerializedProperty gizmoSphereRadius;
        SerializedProperty lineWidth;
        SerializedProperty player;
        SerializedProperty dynamicLength;
        SerializedProperty uvScrollSpeed;
        SerializedProperty checkPoints;
        SerializedProperty checkpointDistanceThreshold;
        SerializedProperty pointsPerSegment;

        void OnEnable()
        {
            toggleGizmos                = serializedObject.FindProperty("ToggleGizmos");
            gizmoSphereRadius           = serializedObject.FindProperty("gizmoSphereRadius");
            lineWidth                   = serializedObject.FindProperty("lineWidth");
            player                      = serializedObject.FindProperty("player");
            dynamicLength               = serializedObject.FindProperty("dynamicLength");
            uvScrollSpeed               = serializedObject.FindProperty("uvScrollSpeed");
            checkPoints                 = serializedObject.FindProperty("checkPoints");
            checkpointDistanceThreshold = serializedObject.FindProperty("checkpointDistanceThreshold");
            pointsPerSegment            = serializedObject.FindProperty("pointsPerSegment");
        }

        public override void OnInspectorGUI()
        {
            GuidanceLine script = (GuidanceLine)target;
            serializedObject.Update();

            // Gizmos
            EditorGUILayout.LabelField("Gizmos Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(toggleGizmos, new GUIContent("Show Gizmos"));
            if (toggleGizmos.boolValue)
                EditorGUILayout.PropertyField(gizmoSphereRadius, new GUIContent("Gizmo Sphere Radius"));

            EditorGUILayout.Space();

            // Line Settings
            EditorGUILayout.LabelField("Line Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(lineWidth, new GUIContent("Line Width"));
            EditorGUILayout.PropertyField(player, new GUIContent("Player (Start Point)"));
            EditorGUILayout.PropertyField(dynamicLength, new GUIContent("Dynamic Length"));
            EditorGUILayout.PropertyField(uvScrollSpeed, new GUIContent("UV Scroll Speed"));

            EditorGUILayout.Space();

            // Checkpoints
            EditorGUILayout.LabelField("Check Points", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(checkPoints, new GUIContent("Check Points Array"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // Checkpoint Settings
            EditorGUILayout.PropertyField(checkpointDistanceThreshold, new GUIContent("Checkpoint Distance Threshold"));
            EditorGUILayout.PropertyField(pointsPerSegment, new GUIContent("Points Per Segment"));

            EditorGUILayout.Space(15f);

            // Visualization Buttons
            if (GUILayout.Button("Visualize Guidance Line"))
                InvokePrivateMethod(script, "EnableVisualization");

            EditorGUILayout.Space(10f);

            if (GUILayout.Button("Stop Visualizing"))
                InvokePrivateMethod(script, "ResetVisualizationState");

            serializedObject.ApplyModifiedProperties();
        }

        private void InvokePrivateMethod(GuidanceLine script, string methodName)
        {
            MethodInfo method = typeof(GuidanceLine).GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(script, null);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
