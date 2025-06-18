using UnityEngine;

public class HammerSwingObstacle : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingAngle = 60f;     // 최대 각도 (좌우)
    public float swingSpeed = 2f;      // 스윙 속도 (진동 주기)
    public float penalty = 10f;        // 충돌 시 패널티

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        // Z축을 기준으로 앞뒤로 흔들리게 만듦 (바이킹 느낌)
        float angle = swingAngle * Mathf.Sin(Time.time * swingSpeed);
        transform.localRotation = startRotation * Quaternion.Euler(angle, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Player hit by hammer! +{penalty} seconds penalty");

            PlayerBehavior pb = collision.gameObject.GetComponent<PlayerBehavior>();
            if (pb != null)
                pb.GetHit(Vector3.up * 5f, penalty);
        }
    }
}
