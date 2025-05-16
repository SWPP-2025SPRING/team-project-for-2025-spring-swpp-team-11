using UnityEngine;

public class BouncingRock : MonoBehaviour
{
    public Transform player;
    public float rad = 5f;
    public float maxHeight = 5f;
    public float minHeight = 0f;
    public float speed = 2f;
    public float interval = 1f;
    public float penalty = 5f;

    private enum State { Idle, Dropping, WaitingAtBottom, Rising, WaitingAtTop, PlayerHit }
    private State currentState = State.Idle;

    private float waitTimer = 0f;
    private Vector3 originalPosition;
    private bool playerInRange = false;
    private bool hasHitPlayer = false;

    void Start()
    {
        originalPosition = transform.position;
        SetPositionY(maxHeight);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        playerInRange = distance <= rad;

        if (!playerInRange && currentState != State.Idle)
        {
            currentState = State.Rising;
            hasHitPlayer = false;
        }

        switch (currentState)
        {
            case State.Idle:
                if (playerInRange)
                {
                    currentState = State.Dropping;
                }
                break;

            case State.Dropping:
                MoveTowardsY(minHeight);
                if (IsAtHeight(minHeight))
                {
                    currentState = State.WaitingAtBottom;
                    waitTimer = 0f;
                }
                break;

            case State.WaitingAtBottom:
                waitTimer += Time.deltaTime;
                if (waitTimer >= interval)
                {
                    currentState = State.Rising;
                }
                break;

            case State.Rising:
                MoveTowardsY(maxHeight);
                if (IsAtHeight(maxHeight))
                {
                    currentState = playerInRange ? State.WaitingAtTop : State.Idle;
                    waitTimer = 0f;
                }
                break;

            case State.WaitingAtTop:
                waitTimer += Time.deltaTime;
                if (waitTimer >= interval)
                {
                    currentState = State.Dropping;
                }
                break;

            case State.PlayerHit:
                MoveTowardsY(player.position.y + 2f); // hover slightly above player
                if (IsAtHeight(player.position.y + 2f))
                {
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= interval)
                    {
                        currentState = State.Rising;
                        waitTimer = 0f;
                    }
                }
                break;
        }
    }

    private void MoveTowardsY(float targetY)
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, targetY, speed * Time.deltaTime);
        transform.position = pos;
    }

    private bool IsAtHeight(float y)
    {
        return Mathf.Abs(transform.position.y - y) < 0.05f;
    }

    private void SetPositionY(float y)
    {
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHitPlayer && collision.gameObject.CompareTag("Player"))
        {
            // Apply penalty logic here (e.g., increase timer)
            Debug.Log("Player hit! Apply penalty time: " + penalty + " seconds");
            currentState = State.PlayerHit;
            waitTimer = 0f;
            hasHitPlayer = true;
        }
    }
}
