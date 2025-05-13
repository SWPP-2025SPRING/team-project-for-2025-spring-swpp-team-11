using UnityEngine;

public class ThrowingMonster : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Transform throwPoint;
    public Transform player;
    public float interval = 3f;
    public float rotationSpeed = 2f;
    public float throwForce = 10f;
    public float penalty = 5f;

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
        direction.y = 0f; // y축 고정
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void ThrowObstacle()
    {
        GameObject obj = Instantiate(obstaclePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        Vector3 direction = (player.position - throwPoint.position).normalized;
        rb.AddForce(direction * throwForce + Vector3.up * 3f, ForceMode.Impulse); // 포물선 경로
    }
}
