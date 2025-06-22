using UnityEditor;
using UnityEngine;

namespace GuidanceLine
{

    /// <summary>
    /// this Script is here to provide a dynamic aspect to the line, to respond to player movements 
    /// </summary>


    [ExecuteInEditMode]
    public class CheckPointMovement : MonoBehaviour
    {
        [Tooltip("Toggle checkPoint Gizmos on or off")]
        [SerializeField]
        private bool drawGizmos = true;

        [Tooltip("Reference your player here")]
        [SerializeField]
        private Transform player;

        [Tooltip("Reference to the next checkPoint in the line")]
        [SerializeField]
        private Transform nextCheckPoint;

        private Vector3 nextCheckPointStartPosition;

        [Tooltip("Dictates the reactivity of the line to the player's movements")]
        [SerializeField]
        private float moveSpeed = 1f;

        [Tooltip("Dictates how far the checkPoint can move away from the position it was placed")]
        [SerializeField]
        private float radius = 1f; // Radius within which the middlepoint can move from its starting position

        private Vector3 startingPosition;
        private Vector3 midpoint;

        void Start()
        {
            if (Application.isPlaying)
            {
                GetNextObjectStartingPosition();
                InitializeStartingPosition();
                CalculateAndSetMidpointPosition();
            }
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                if (nextCheckPoint == null)
                {
                    return;
                }

                UpdateMidpoint();
                MoveTowardsMidpoint();
            }
            else if (!Application.isPlaying) // update the gizmos in the editor
            {
                InitializeStartingPosition();
            }
        }



        private void GetNextObjectStartingPosition()
        {
            if (nextCheckPoint != null)
            {
                var nextCheckPointMovement = nextCheckPoint.gameObject.GetComponent<CheckPointMovement>();
                if (nextCheckPointMovement == null)
                {
                    return;
                }
                nextCheckPointStartPosition = nextCheckPoint.gameObject.GetComponent<CheckPointMovement>().GetStartPosition();
            }
        }

        private void InitializeStartingPosition()
        {
            startingPosition = transform.position;
        }

        private void CalculateAndSetMidpointPosition()
        {
            UpdateMidpoint();
            MoveToMidpointDirectly();
        }

        private void UpdateMidpoint()
        {
            midpoint = (player.position + nextCheckPointStartPosition) / 2f;
        }

        private void MoveToMidpointDirectly()
        {
            Vector3 directionToMidpoint = (midpoint - transform.position).normalized;
            Vector3 targetPosition = GetTargetPosition(directionToMidpoint);
            transform.position = ClampPositionWithinRadius(targetPosition);
        }

        private void MoveTowardsMidpoint()
        {
            Vector3 directionToMidpoint = (midpoint - transform.position).normalized;
            Vector3 targetPosition = GetTargetPosition(directionToMidpoint, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(startingPosition, targetPosition) <= radius)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = ClampPositionWithinRadius(targetPosition);
            }
        }

        private Vector3 GetTargetPosition(Vector3 direction, float scale = 1f)
        {
            Vector3 targetPosition = transform.position + direction * scale;
            targetPosition.y = transform.position.y; // Keep Y constant
            return targetPosition;
        }

        private Vector3 ClampPositionWithinRadius(Vector3 targetPosition)
        {
            Vector3 clampedPosition = startingPosition + (targetPosition - startingPosition).normalized * radius;
            clampedPosition.y = startingPosition.y; // Ensure Y remains constant
            return clampedPosition;
        }

        private void UpdateStartingPositionForEditor() { startingPosition = transform.position; }

        public Vector3 GetStartPosition() { return startingPosition; }

        public void SetGizmos(bool result) { drawGizmos = result; }

        public void SetPlayer(Transform _player) { this.player = _player; }

        private void OnDisable() { this.gameObject.transform.position = startingPosition; }

        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(startingPosition, radius);
            }
        }

    }
}

