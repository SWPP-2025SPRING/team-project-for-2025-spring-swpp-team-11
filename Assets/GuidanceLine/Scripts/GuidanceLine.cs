using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GuidanceLine
{

    public class GuidanceLine : MonoBehaviour
    {
        [Tooltip("Toggle Line Gizmos on or off")]
        public bool ToggleGizmos = true;
        public float gizmoSphereRadius = 0.1f; // Radius of the gizmo spheres

        [Tooltip("Reference your player here")]
        public Transform startPoint; // The start of the line

        [Tooltip("Reference the target location")]
        public Transform endPoint; // The end of the line

        [Tooltip("The points the player will have to go through to reach the target location. Add and remove as needed")]
        public Transform[] checkPoints; // all the checkpoints you have to go through as you progress

        private LineRenderer lineRenderer;

        [Tooltip("Adjust line width as needed")]
        [SerializeField]
        private float lineWidth = 0.05f; // adjust line width depending on your projects

        [Tooltip("Adjust the number of points in between each checkPoint. More points = smoother line")]
        [SerializeField]
        private int pointsPerSegment = 20;

        [Tooltip("Dstance to consider the checkpoint as reached")]
        [SerializeField]
        private float checkpointDistanceThreshold = 1f; //distance to consider the checkpoint as reached

        public List<Vector3[]> segments = new List<Vector3[]>();
        private int minPointsPerSegment = 10; // Minimum number of points per segment, adjust as needed
        private int maxPointsPerSegment;
        private Transform activecheckPoint;
        private Vector3[] allPoints;
        private Vector3[] firstSegmentPoints;

        //Editor resource
        [SerializeField]
        private int checkPointNumber = 0;

        void Start()
        {
            InitializeLine();
        }

        void InitializeLine()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = lineWidth;
            segments.Clear();
            if (pointsPerSegment < minPointsPerSegment) { pointsPerSegment = minPointsPerSegment; } // override when there is not enought points
            maxPointsPerSegment = pointsPerSegment;
            activecheckPoint = (checkPoints.Length == 0) ? endPoint : checkPoints[0];
            DrawCurvedLine();
        }

        void Update()
        {
            DrawCurvedLine();
            float distanceToFirstCheckPoint;
            activecheckPoint = (checkPoints.Length > 0) ? checkPoints[0] : endPoint;
            distanceToFirstCheckPoint = (checkPoints.Length == 0) ? Vector3.Distance(startPoint.position, endPoint.position) : Vector3.Distance(startPoint.position, checkPoints[0].position);

            // if we get close enough to a checkPoint or the endPoint
            if (distanceToFirstCheckPoint <= checkpointDistanceThreshold)
            {
                RemoveActiveSegment();
            }
        }

        void DrawCurvedLine()
        {
            if (startPoint == null || endPoint == null || lineRenderer == null)
                return;

            // Calculate the total number of segments and points
            int segmentCount = checkPoints.Length + 1;
            int totalPoints = (pointsPerSegment - 1) * segmentCount + 1; // Avoid overestimating
            allPoints = new Vector3[totalPoints];

            // Prepare the path points
            Vector3[] pathPoints = new Vector3[checkPoints.Length + 2];
            pathPoints[0] = startPoint.position;
            for (int i = 0; i < checkPoints.Length; i++)
            {
                pathPoints[i + 1] = checkPoints[i].position;
            }
            pathPoints[pathPoints.Length - 1] = endPoint.position;

            segments.Clear();
            int index = 0;

            // Generate points for each segment
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Vector3 p0 = i == 0 ? pathPoints[i] : pathPoints[i - 1];
                Vector3 p1 = pathPoints[i];
                Vector3 p2 = pathPoints[i + 1];
                Vector3 p3 = i == pathPoints.Length - 2 ? pathPoints[i + 1] : pathPoints[i + 2];

                int segmentPointCount = (i == 0) ? CalculateDynamicPointCount() : pointsPerSegment;
                Vector3[] segmentPoints = new Vector3[segmentPointCount];

                for (int j = 0; j < segmentPointCount; j++)
                {
                    float t = (float)j / (segmentPointCount - 1);
                    Vector3 pointOnCurve = CatmullRom(p0, p1, p2, p3, t);

                    // Add points to the array and segment list
                    segmentPoints[j] = pointOnCurve;
                    if (index < allPoints.Length)
                    {
                        allPoints[index++] = pointOnCurve;
                    }
                }
                segments.Add(segmentPoints);
            }

            // Update the LineRenderer
            lineRenderer.positionCount = index; // Only use the points added
            lineRenderer.SetPositions(allPoints);
        }

        // Function to dynamically calculate the point count for the active segment
        int CalculateDynamicPointCount()
        {
            float distance;
            if (checkPoints.Length == 0)// handles the case where there is no more check points because we're at the endPoint
            {
                float distanceEnd = Vector3.Distance(startPoint.position, endPoint.position);
                distance = distanceEnd;
            }
            else
            {
                float distanceOnGoing = Vector3.Distance(startPoint.position, checkPoints[0].position);
                distance = distanceOnGoing;
            }
            int dynamicPointCount = Mathf.FloorToInt(maxPointsPerSegment * (distance / 5));
            return Mathf.Clamp(dynamicPointCount, minPointsPerSegment, maxPointsPerSegment);
        }

        //When a checkpoint is reached, remove the active segment and draw and new line towards the next checkPoint
        void RemoveActiveSegment()
        {
            if (checkPoints.Length == 0)
            {
                EndLine();
                return;
            }

            // Create a new array that's one element smaller
            Transform[] newCheckPoints = new Transform[checkPoints.Length - 1];

            // Copy the elements, starting from index 1 to the end of the original array
            for (int i = 1; i < checkPoints.Length; i++)
            {
                newCheckPoints[i - 1] = checkPoints[i];
            }

            // Assign the new array back to checkPoints
            checkPoints = newCheckPoints;

            // Update the activeCheckPoint to the new first element if any remain
            activecheckPoint = (checkPoints.Length > 0) ? checkPoints[0] : endPoint;
        }

        // Method used to calculate position along the Catmull-Rom spline
        Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2.0f * p1) +
                (-p0 + p2) * t +
                (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
                (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
        }

        // Method for visualization, Debugging
        void OnDrawGizmos()
        {
            if (ToggleGizmos == false || lineRenderer == null) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Gizmos.DrawWireSphere(lineRenderer.GetPosition(i), gizmoSphereRadius);
            }
        }

        // Method to handle the completion of the GuidanceLine
        private void EndLine()
        {
            ToggleGizmos = false;
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            checkPointNumber++; // this is really just to remove a warning
            this.enabled = false;
        }

        // Custom Editor functions

        private void ResetVisualizationState()
        {
            lineRenderer.positionCount = 0;
            foreach (Transform point in checkPoints)
            {
                point.GetComponent<CheckPointMovement>().SetGizmos(false);
            }
        }

        void EnableVisualization()
        {
            if (ToggleGizmos)
            {
                foreach (Transform point in checkPoints)
                {
                    point.gameObject.GetComponent<CheckPointMovement>().SetGizmos(true);
                }
            }
            InitializeLine();
        }


    }
}
