using System.Collections.Generic;
using UnityEngine;

namespace GuidanceLine
{
    [RequireComponent(typeof(LineRenderer))]
    public class GuidanceLine : MonoBehaviour
    {
        [Header("Dynamic Start")]
        [Tooltip("Player transform – dynamic starting point")]
        public Transform player;

        [Header("Checkpoints")]
        [Tooltip("Ordered array of checkpoint transforms")]
        public Transform[] checkPoints;

        [Header("Line Settings")]
        [Tooltip("Line width for renderer")]
        public float lineWidth = 0.1f;
        [Tooltip("Number of interpolated points per segment")]
        public int pointsPerSegment = 20;

        [Header("Gizmos")]
        [Tooltip("Show debug gizmos")]
        public bool ToggleGizmos = true;
        [Tooltip("Radius of gizmo spheres")]
        public float gizmoSphereRadius = 0.1f;

        private LineRenderer lr;
        private int nextCheckpointIndex = 0;

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.startWidth = lineWidth;
            lr.positionCount = 0;
        }

        void Update()
        {
            if (player == null || lr == null || checkPoints == null || checkPoints.Length == 0)
                return;

            // Plane crossing method for skipping passed checkpoints
            // We define a plane at the checkpoint perpendicular to the direction towards the next checkpoint
            if (nextCheckpointIndex < checkPoints.Length)
            {
                Vector3 currentPos = checkPoints[nextCheckpointIndex].position;
                Vector3 dir;
                if (nextCheckpointIndex < checkPoints.Length - 1)
                {
                    // direction to next checkpoint
                    dir = (checkPoints[nextCheckpointIndex + 1].position - currentPos).normalized;
                }
                else
                {
                    // last checkpoint: use player forward to define plane
                    dir = player.forward;
                }

                // Dot > 0 means player passed the plane
                float dp = Vector3.Dot(player.position - currentPos, dir);
                if (dp > 0f)
                {
                    nextCheckpointIndex++;
                }
            }

            DrawLine();
        }

        void DrawLine()
        {
            // Build list of points: start with player, then remaining checkpoints
            List<Vector3> pts = new List<Vector3> { player.position };
            for (int i = nextCheckpointIndex; i < checkPoints.Length; i++)
            {
                if (checkPoints[i] != null)
                    pts.Add(checkPoints[i].position);
            }

            // Nothing to draw if fewer than two points
            if (pts.Count < 2)
            {
                lr.positionCount = 0;
                return;
            }

            int segments = pts.Count - 1;
            int totalPts = segments * pointsPerSegment + 1;
            Vector3[] positions = new Vector3[totalPts];
            int idx = 0;

            // Catmull-Rom interpolation per segment
            for (int s = 0; s < segments; s++)
            {
                Vector3 p0 = (s == 0) ? pts[s] : pts[s - 1];
                Vector3 p1 = pts[s];
                Vector3 p2 = pts[s + 1];
                Vector3 p3 = (s + 2 < pts.Count) ? pts[s + 2] : pts[s + 1];

                for (int j = 0; j < pointsPerSegment; j++)
                {
                    float t = j / (float)(pointsPerSegment - 1);
                    positions[idx++] = CatmullRom(p0, p1, p2, p3, t);
                }
            }

            // Append final endpoint once
            positions[idx++] = pts[pts.Count - 1];

            lr.positionCount = idx;
            lr.SetPositions(positions);
        }

        Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * ((2f * p1) +
                           (-p0 + p2) * t +
                           (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                           (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }

        void OnDrawGizmos()
        {
            if (!ToggleGizmos || lr == null)
                return;

            Gizmos.color = Color.red;
            for (int i = 0; i < lr.positionCount; i++)
                Gizmos.DrawWireSphere(lr.GetPosition(i), gizmoSphereRadius);
        }
    }
}
