using UnityEditor;
using UnityEngine;

namespace GuidanceLine
{

    /// <summary>
    /// 
    /// This is the custom editor for the GuidanceLine, bringing ease-of-use and a cleaner interface
    /// plus some nifty buttons to visualize and debug you line
    /// 
    /// </summary>


    [CustomEditor(typeof(GuidanceLine))]
    public class GuidanceLineCustomEditor : Editor
    {
        SerializedProperty toggleGizmos;
        SerializedProperty gizmoSphereRadius;
        SerializedProperty lineWidth;
        SerializedProperty startPoint;
        SerializedProperty checkPoints;
        SerializedProperty endPoint;
        SerializedProperty checkpointDistanceThreshold;
        SerializedProperty pointsPerSegment;

        void OnEnable()
        {
            toggleGizmos = serializedObject.FindProperty("ToggleGizmos");
            gizmoSphereRadius = serializedObject.FindProperty("gizmoSphereRadius");
            lineWidth = serializedObject.FindProperty("lineWidth");
            startPoint = serializedObject.FindProperty("startPoint");
            checkPoints = serializedObject.FindProperty("checkPoints");
            endPoint = serializedObject.FindProperty("endPoint");
            checkpointDistanceThreshold = serializedObject.FindProperty("checkpointDistanceThreshold");
            pointsPerSegment = serializedObject.FindProperty("pointsPerSegment");
        }

        public override void OnInspectorGUI()
        {
            GuidanceLine script = (GuidanceLine)target;
            serializedObject.Update();

            // Toggle Gizmos Header
            EditorGUILayout.LabelField("Gizmos Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(toggleGizmos);

            // Show gizmo size field only if ToggleGizmos is true
            if (toggleGizmos.boolValue)
            {
                EditorGUILayout.PropertyField(gizmoSphereRadius);
            }

            EditorGUILayout.Space();

            // Start Point Header
            EditorGUILayout.LabelField("Line Settings", EditorStyles.boldLabel);
            // Line Width
            EditorGUILayout.PropertyField(lineWidth, new GUIContent("Line Width"));
            // Start Point
            EditorGUILayout.PropertyField(startPoint, new GUIContent("Start Point"));
            // End Point
            EditorGUILayout.PropertyField(endPoint, new GUIContent("End Point"));

            EditorGUILayout.Space();

            // check Point List
            EditorGUI.indentLevel++;
            SerializedProperty checkPoints = serializedObject.FindProperty("checkPoints");
            EditorGUILayout.PropertyField(checkPoints, true);
            EditorGUI.indentLevel--;

            // Add/Remove Buttons for check Points
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Check Point"))
            {
                AddCheckPoint(script);
            }
            if (GUILayout.Button("Remove Check Point"))
            {
                RemoveCheckPoint(script);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //checkpoint Distance Threshold
            EditorGUILayout.PropertyField(checkpointDistanceThreshold, new GUIContent("Checkpoint Distance Threshold"));

            // Points Per Segment
            EditorGUILayout.PropertyField(pointsPerSegment, new GUIContent("Points Per Segment"));

            EditorGUILayout.Space(15f);

            // Add "Reset Line Points" button at the bottom
            if (GUILayout.Button("Visualize Guidance Line"))
            {
                VisualizeGuidanceLine(script);
            }

            EditorGUILayout.Space(10f);

            // Add "Reset Line Points" button at the bottom
            if (GUILayout.Button("Stop Visualizing"))
            {
                StopVisualizingGuidanceLine(script);
            }

            // Apply changes to serialized object
            serializedObject.ApplyModifiedProperties();
        }

        private void AddCheckPoint(GuidanceLine script)
        {
            serializedObject.Update();
            Undo.RecordObject(script, "Add Check Point");


            SerializedProperty checkPointNumberProperty = serializedObject.FindProperty("checkPointNumber");
            checkPointNumberProperty.intValue++;

            GameObject newPoint = new GameObject("CheckPoint" + checkPointNumberProperty.intValue);

            newPoint.transform.position = Vector3.zero;
            newPoint.transform.parent = ((GuidanceLine)target).transform;

            SerializedProperty checkPointsProperty = serializedObject.FindProperty("checkPoints");
            checkPointsProperty.arraySize++;
            checkPointsProperty.GetArrayElementAtIndex(checkPointsProperty.arraySize - 1).objectReferenceValue = newPoint.transform;

            //Add the check point movement script + populate it
            CheckPointMovement script2 = newPoint.AddComponent<CheckPointMovement>();
            script2.SetPlayer(script.startPoint);
            // make it do other things if you need here


            serializedObject.ApplyModifiedProperties();

            RepositionCheckPoints(script);
        }

        private void RemoveCheckPoint(GuidanceLine script)
        {
            serializedObject.Update();
            Undo.RecordObject(script, "Remove Check Point");

            SerializedProperty checkPointNumberProperty = serializedObject.FindProperty("checkPointNumber");
            SerializedProperty checkPointsProperty = serializedObject.FindProperty("checkPoints");

            //this section deals with deleted elements (potential null elements in the array)
            int realCount = 0;
            for (int i = 0; i < checkPointsProperty.arraySize; i++)
            {
                if (checkPointsProperty.GetArrayElementAtIndex(i).objectReferenceValue != null) { realCount++; }
                else { checkPointsProperty.DeleteArrayElementAtIndex(i); }
                checkPointsProperty.serializedObject.ApplyModifiedProperties();
            }
            // Now realCount holds the number of non-null elements
            int lastIndex = realCount - 1;

            if (lastIndex >= 0)
            {
                checkPointNumberProperty.intValue--;

                //Get the last check point and remove it from the array
                Transform lastPoint = (Transform)checkPointsProperty.GetArrayElementAtIndex(lastIndex).objectReferenceValue;
                checkPointsProperty.DeleteArrayElementAtIndex(lastIndex);


                // Destroy the GameObject (only in editor mode)
                if (lastPoint != null)
                {
                    if (Application.isEditor && !Application.isPlaying)
                    {
                        DestroyImmediate(lastPoint.gameObject);
                    }
                    else
                    {
                        Destroy(lastPoint.gameObject);
                    }
                }
                serializedObject.ApplyModifiedProperties();
                RepositionCheckPoints(script); // remove this if you don't want the checkPoint ot be proportionally placed along the line
            }
            else
            {
                checkPointNumberProperty.intValue = 0;
            }
        }

        private void RepositionCheckPoints(GuidanceLine script)
        {
            if (script.startPoint == null || script.endPoint == null)
                return;

            Vector3 startPosition = script.startPoint.position;
            Vector3 endPosition = script.endPoint.position;

            // Reposition each check point proportionally between the start and end point
            int checkPointCount = script.checkPoints.Length;
            for (int i = 0; i < checkPointCount; i++)
            {
                float t = (float)(i + 1) / (checkPointCount + 1);
                script.checkPoints[i].position = Vector3.Lerp(startPosition, endPosition, t);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }


        // Visualisation and debugging stuff
        private void VisualizeGuidanceLine(GuidanceLine script)
        {
            var type = script.GetType();
            var resetMethod = type.GetMethod("EnableVisualization", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (resetMethod != null)
            {
                resetMethod.Invoke(script, null);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void StopVisualizingGuidanceLine(GuidanceLine script)
        {
            var type = script.GetType();
            var resetMethod = type.GetMethod("ResetVisualizationState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (resetMethod != null)
            {
                resetMethod.Invoke(script, null);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}
