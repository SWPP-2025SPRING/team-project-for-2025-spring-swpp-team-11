using UnityEngine;

public class FanBladeObstacle : MonoBehaviour
{
    public float speed = 180f; // 회전 속도 (도/초)
    public float height = 1f;  // 선풍기 날이 위치할 고정 높이
    public float penalty = 10f;

    public Transform player;

    private void Start()
    {
        // 장애물 높이 고정
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        // Rigidbody가 있다면, 회전/이동 고정
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void Update()
    {
        // Y축 기준 회전
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Knockback, 스턴 등 추가할 수 있음
            Debug.Log("Player hit by fan blade! +" + penalty + "s penalty.");

            // 예시: PlayerBehavior 스크립트에서 처리
            // collision.gameObject.GetComponent<PlayerBehavior>()?.ApplyPenalty(penalty);
        }
    }
}
