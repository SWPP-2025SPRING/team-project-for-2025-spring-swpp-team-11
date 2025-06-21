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

        // Tracks which checkpoints have been passed
        private HashSet<Transform> passed = new HashSet<Transform>();
        private LineRenderer lr;
        private int nextCheckpointIndex = 0;

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.startWidth = lineWidth;
            lr.positionCount = 0;

            if (player == null)
            {
                var go = GameObject.FindWithTag("Player");
                if (go != null)
                    player = go.transform;
            }
        }

        void OnEnable()
        {
            passed.Clear();
            nextCheckpointIndex = 0;
        }

        /// <summary>
        /// Call this from a trigger on the checkpoint when player passes it
        /// </summary>
        public void MarkPassed(Transform checkpoint)
        {
            if (!passed.Contains(checkpoint))
                passed.Add(checkpoint);
        }

        void Update()
        {
            if (player == null || lr == null || checkPoints == null || checkPoints.Length == 0)
                return;

            DrawLine();
        }

        /// <summary>
        /// Resets the guidance so that all checkpoints are redrawn from the start.
        /// </summary>
        public void ResetGuidance()
        {
            // Clear all passed checkpoints and start from index 0
            passed.Clear();
            nextCheckpointIndex = 0;
            DrawLine();
        }

        /// <summary>
        /// Resets the guidance, marking all checkpoints up to startIndex as passed,
        /// and begins drawing from that point onward.
        /// </summary>
        public void ResetGuidance(int startIndex)
        {
            passed.Clear();
            // Clamp to valid range
            nextCheckpointIndex = Mathf.Clamp(startIndex, 0, checkPoints.Length);
            // Mark earlier checkpoints as passed
            for (int i = 0; i < nextCheckpointIndex+1; i++)
            {
                if (checkPoints[i] != null)
                    passed.Add(checkPoints[i]);
            }
            DrawLine();
        }

        void DrawLine()
        {
            // Build list: player position + only checkpoints not yet passed
            List<Vector3> pts = new List<Vector3> { player.position };
            foreach (var cp in checkPoints)
            {
                if (cp != null && !passed.Contains(cp))
                    pts.Add(cp.position);
            }

            int n = pts.Count;
            if (n < 2)
            {
                lr.positionCount = 0;
                return;
            }

            int segments = n - 1;
            int totalPts = segments * pointsPerSegment + 1;
            Vector3[] positions = new Vector3[totalPts];
            int idx = 0;

            // Catmull-Rom interpolation across each segment
            for (int s = 0; s < segments; s++)
            {
                Vector3 p0 = (s == 0) ? pts[0] : pts[s - 1];
                Vector3 p1 = pts[s];
                Vector3 p2 = pts[s + 1];
                Vector3 p3 = (s + 2 < n) ? pts[s + 2] : pts[s + 1];

                for (int j = 0; j < pointsPerSegment; j++)
                {
                    float t = j / (float)(pointsPerSegment - 1);
                    positions[idx++] = CatmullRom(p0, p1, p2, p3, t);
                }
            }

            // Append final endpoint
            positions[idx++] = pts[n - 1];

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
