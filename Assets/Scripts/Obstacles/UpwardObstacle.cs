using UnityEngine;

public class UpwardObstacle : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;
    public float detectionRadius = 3f;
    public float detectionHeightOffset = 0.5f;

    [Header("Launch Settings")]
    public float launchForce = 10f;
    public float interval = 1f;
    public float penalty = 5f;

    [Header("References")]
    public Rigidbody rb;
    public UpwardObstacleSpawner spawner;
    public string playerTag = "Player";

    private enum State { Idle, Launched, WaitingToRespawn }
    private State currentState = State.Idle;

    private bool hasHitPlayer = false;
    private float timer = 0f;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                if (IsPlayerDirectlyAbove())
                {
                    Launch();
                }
                break;

            case State.Launched:
                if (rb.linearVelocity.y < 0f && transform.position.y <= spawner.spawnPoint.position.y + 0.05f)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.isKinematic = true;
                    rb.useGravity = false;

                    if (spawner != null)
                        spawner.TriggerRespawn(interval);

                    Destroy(gameObject);
                }
                break;


        }
    }

    private void Launch()
    {
        currentState = State.Launched;
        hasHitPlayer = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero; // Reset before force
        rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);
    }

    private bool IsPlayerDirectlyAbove()
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector2 flatOffset = new Vector2(toPlayer.x, toPlayer.z);

        bool inRadius = flatOffset.magnitude <= detectionRadius;
        bool isAbove = toPlayer.y > detectionHeightOffset;

        return inRadius && isAbove;
    }
    
    // For Test
    public bool IsPlayerDirectlyAbove(Vector3 playerPos, Vector3 thisPos)
    {
        Vector3 toPlayer = playerPos - thisPos;
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

            Debug.Log($"Player hit by lava plume! +{penalty} seconds penalty");

            PlayerBehavior pb = collision.gameObject.GetComponent<PlayerBehavior>();
            if (pb != null)
            {
                pb.GetHit(Vector3.up * 5f, penalty);
            }

            if (spawner != null)
                spawner.TriggerRespawn(interval);

            Destroy(gameObject);
        }
    }
}
