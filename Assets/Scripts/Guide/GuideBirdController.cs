using UnityEngine;
using System.Collections;

public class GuideBirdController : MonoBehaviour
{
    [Tooltip("플레이어 Transform (태그가 'Player'인 오브젝트를 자동 할당)")]
    public Transform player;

    [Tooltip("플레이어로부터 최대 허용 반경")]
    public float maxDistanceFromPlayer = 10f;

    [Tooltip("이동할 목적지들 (빈 GameObject)")]
    public Transform[] waypoints;
    private int currentIndex = 0;

    [Tooltip("이동 속도")]
    public float flySpeed = 5f;

    [Tooltip("회전 속도")]
    public float turnSpeed = 360f;

    private Transform targetPoint;
    private bool isFlying = false;

    void Start()
    {
        // 플레이어를 할당하지 않았다면 태그로 찾아 자동 세팅
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) player = go.transform;
        }

        if (waypoints.Length > 0)
        {
            targetPoint = waypoints[0];
            FaceTargetInstant();
            isFlying = true;
        }
    }

    void Update()
    {
        if (!isFlying || targetPoint == null || player == null) return;

        // 1) 부드럽게 회전
        Vector3 dir = (targetPoint.position - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // 2) 이동
        transform.position += transform.forward * flySpeed * Time.deltaTime;

        // 3) 플레이어 반경 안에 머무르도록 위치 보정
        Vector3 toBird = transform.position - player.position;
        float dist = toBird.magnitude;
        if (dist > maxDistanceFromPlayer)
        {
            // 반경 바깥으로 나갔으면 플레이어 주변 경계점으로 되돌리기
            Vector3 clamped = player.position + toBird.normalized * maxDistanceFromPlayer;
            transform.position = clamped;
        }

        // player보다 7만큼 더 높게 고정
        Vector3 pos = transform.position;
        pos.y = player.position.y + 7f;
        transform.position = pos;
    }

    // Trigger에서 호출할 메서드
    public void GoToNextPoint()
    {
        currentIndex++;
        if (currentIndex >= waypoints.Length)
        {
            // 마지막 도착
            isFlying = false;
            return;
        }
        targetPoint = waypoints[currentIndex];
        // 즉시 회전 방향만 바꾸고 싶다면:
        FaceTargetInstant();
        isFlying = true;
    }

    // 즉시 바라보기
    private void FaceTargetInstant()
    {
        Vector3 dir = (targetPoint.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}
