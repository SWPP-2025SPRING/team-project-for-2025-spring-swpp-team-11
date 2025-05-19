using UnityEngine;

public class BouncingRock : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;
    public float detectionRadius = 5f;
    public float detectionHeightOffset = 0.5f;

    [Header("Motion Settings")]
    public float interval = 1f;
    public float penalty = 5f;

    [Header("References")]
    public Rigidbody rb;
    public FallingObstacleSpawner spawner;

    private enum State { Idle, Dropping, WaitingAtBottom, Rising, WaitingAtTop }
    private State currentState = State.Idle;

    private float waitTimer = 0f;
    private Vector3 startPosition;
    private bool hasHitPlayer = false;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        startPosition = transform.position;

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                if (IsPlayerDirectlyBelow())
                    StartDrop();
                break;

            case State.WaitingAtBottom:
                waitTimer += Time.deltaTime;
                if (waitTimer >= interval)
                    StartRising();
                break;

            case State.WaitingAtTop:
                waitTimer += Time.deltaTime;
                if (waitTimer >= interval)
                    currentState = State.Idle;
                break;
        }
    }

    private bool IsPlayerDirectlyBelow()
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector2 flatOffset = new Vector2(toPlayer.x, toPlayer.z);

        bool inRadius = flatOffset.magnitude <= detectionRadius;
        bool isUnder = toPlayer.y < -detectionHeightOffset;

        return inRadius && isUnder;
    }

    private void StartDrop()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        currentState = State.Dropping;
        hasHitPlayer = false;
    }

    private void StartRising()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        transform.position = startPosition;
        currentState = State.WaitingAtTop;
        waitTimer = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == State.Dropping && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            currentState = State.WaitingAtBottom;
            waitTimer = 0f;
        }

        if (!hasHitPlayer && collision.gameObject.CompareTag("Player"))
        {
            hasHitPlayer = true;

            Debug.Log($"Player hit! +{penalty} seconds penalty");

            PlayerBehavior p = collision.gameObject.GetComponent<PlayerBehavior>();
            if (p != null)
                p.GetHit(Vector3.up * 5f, penalty);

            if (spawner != null)
                spawner.TriggerRespawn(interval);

            Destroy(gameObject);
        }
    }
}
