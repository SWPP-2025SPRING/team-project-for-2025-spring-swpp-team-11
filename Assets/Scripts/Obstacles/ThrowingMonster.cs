using UnityEngine;

public class ThrowingMonster : MonoBehaviour
{
    [Header("References")]
    public GameObject obstaclePrefab;
    public Transform throwPoint;
    public Transform player;

    [Header("Throw Settings")]
    public float interval = 3f;              // Time between throws
    public float rotationSpeed = 2f;         // Monster turns toward player
    public float flightTime = 1.5f;          // Time for projectile to reach target
    public float penalty = 5f;               // Penalty applied to player

    private float timer = 0f;

    void Update()
    {
        RotateTowardsPlayer();

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            ThrowObstacle();
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f; // Only rotate on Y-axis
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void ThrowObstacle()
    {
        GameObject obj = Instantiate(obstaclePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        Vector3 velocity = CalculateVelocityToHitTarget(throwPoint.position, player.position, flightTime);

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = velocity;
    }

    Vector3 CalculateVelocityToHitTarget(Vector3 origin, Vector3 target, float time)
    {
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);

        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;
        float gravity = Mathf.Abs(Physics.gravity.y);

        float vY = y / time + 0.5f * gravity * time;
        float vXZ = xz / time;

        Vector3 result = toTargetXZ.normalized * vXZ;
        result.y = vY;

        return result;
    }
}
