using UnityEditor;
using UnityEngine;

namespace GuidanceLine
{
    [CustomEditor(typeof(GuidanceLine))]
    public class GuidanceLineCustomEditor : Editor
    {
        SerializedProperty playerProp;
        SerializedProperty checkPointsProp;
        SerializedProperty lineWidthProp;
        SerializedProperty pointsPerSegmentProp;
        SerializedProperty toggleGizmosProp;
        SerializedProperty gizmoSphereRadiusProp;

        void OnEnable()
        {
            playerProp             = serializedObject.FindProperty("player");
            checkPointsProp        = serializedObject.FindProperty("checkPoints");
            lineWidthProp          = serializedObject.FindProperty("lineWidth");
            pointsPerSegmentProp   = serializedObject.FindProperty("pointsPerSegment");
            toggleGizmosProp       = serializedObject.FindProperty("ToggleGizmos");
            gizmoSphereRadiusProp  = serializedObject.FindProperty("gizmoSphereRadius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Dynamic Start
            EditorGUILayout.LabelField("Dynamic Start", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playerProp, new GUIContent("Player (Start)"));

            // Checkpoints
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Checkpoints", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(checkPointsProp, new GUIContent("Check Points Array"), true);

            // Line Settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Line Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(lineWidthProp,        new GUIContent("Line Width"));
            EditorGUILayout.PropertyField(pointsPerSegmentProp, new GUIContent("Points Per Segment"));

            // Gizmos
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gizmos Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(toggleGizmosProp,      new GUIContent("Show Gizmos"));
            if (toggleGizmosProp.boolValue)
                EditorGUILayout.PropertyField(gizmoSphereRadiusProp, new GUIContent("Gizmo Sphere Radius"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
