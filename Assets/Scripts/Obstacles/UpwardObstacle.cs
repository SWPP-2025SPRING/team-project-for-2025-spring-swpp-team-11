using UnityEngine;

public class UpwardObstacle : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;
    public float detectionRadius = 3f;
    public float detectionHeightOffset = 0.5f;

    [Header("Motion Settings")]
    public float riseHeight = 3f;
    public float riseSpeed = 5f;
    public float interval = 1f;

    public UpwardObstacleSpawner spawner;

    [Header("Impact Settings")]
    public float penalty = 5f;
    public string playerTag = "Player";

    private enum State { Idle, Rising, AtTop, Falling }
    private State currentState = State.Idle;

    private float waitTimer = 0f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool hasHitPlayer = false;
    
    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * riseHeight;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                if (IsPlayerDirectlyAbove())
                {
                    currentState = State.Rising;
                    hasHitPlayer = false;
                }
                break;

            case State.Rising:
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, riseSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                {
                    currentState = State.AtTop;
                    waitTimer = 0f;
                }
                break;

            case State.AtTop:
                waitTimer += Time.deltaTime;
                if (waitTimer >= interval)
                {
                    currentState = State.Falling;
                }
                break;

            case State.Falling:
                transform.position = Vector3.MoveTowards(transform.position, startPosition, riseSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, startPosition) < 0.05f)
                {
                    currentState = State.Idle;
                }
                break;
        }
    }

    private bool IsPlayerDirectlyAbove()
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector2 flatOffset = new Vector2(toPlayer.x, toPlayer.z);

        bool inRadius = flatOffset.magnitude <= detectionRadius;
        bool isAbove = toPlayer.y > detectionHeightOffset;

        return inRadius && isAbove;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHitPlayer && collision.gameObject.CompareTag(playerTag))
        {
            hasHitPlayer = true;
            Debug.Log($"Player hit by upward obstacle! +{penalty} seconds penalty");

            PlayerBehavior pb = collision.gameObject.GetComponent<PlayerBehavior>();
            if (pb != null)
            {
                pb.GetHit(Vector3.up * 5f, penalty);
            }

            Destroy(gameObject);
        }
    }
}
