using UnityEngine;
using System.Collections;

public class GuideBirdController : MonoBehaviour
{
    [Tooltip("플레이어 Transform (태그가 'Player'인 오브젝트를 자동 할당)")]
    public Transform player;

    [Tooltip("이동할 목적지들 (빈 GameObject)")]
    public Transform[] waypoints;

    [Tooltip("이동 속도")]
    public float flySpeed = 5f;

    [Tooltip("회전 속도 (degree/sec)")]
    public float turnSpeed = 360f;

    [Tooltip("높이 오프셋 (플레이어보다 위로)")]
    public float heightOffset = 7f;

    private int currentIndex = 0;
    private Transform targetPoint;
    private bool isFlying = false;

    // Flag to skip movement on the frame of reset
    private bool skipNextUpdate = false;

    void Start()
    {
        // 자동으로 플레이어 참조
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) player = go.transform;
        }

        // 초기 웨이포인트 설정
        if (waypoints != null && waypoints.Length > 0)
        {
            currentIndex = 0;
            targetPoint = waypoints[0];
            FaceTargetInstant();
            isFlying = true;
        }
    }

    void Update()
    {
        if (skipNextUpdate)
        {
            // First update after reset: skip movement
            skipNextUpdate = false;
            return;
        }

        if (!isFlying || targetPoint == null || player == null)
            return;

        // 회전
        Vector3 dir = (targetPoint.position - transform.position);
        float maxRad = turnSpeed * Mathf.Deg2Rad * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, dir.normalized, maxRad, 0f);
        transform.rotation = Quaternion.LookRotation(newDir);

        // 이동
        transform.position += transform.forward * flySpeed * Time.deltaTime;
    }

    /// <summary>
    /// 다음 웨이포인트로 전환을 호출
    /// </summary>
    public void GoToNextPoint()
    {
        currentIndex++;
        if (waypoints == null || currentIndex >= waypoints.Length)
        {
            isFlying = false;
            return;
        }

        targetPoint = waypoints[currentIndex];
        FaceTargetInstant();
        isFlying = true;
    }

    /// <summary>
    /// Reset bird path to beginning (index 0)
    /// </summary>
    public void ResetGuidance()
    {
        ResetGuidance(0);
    }

    /// <summary>
    /// Reset bird and teleport to given waypoint index
    /// </summary>
    public void ResetGuidance(int startIndex)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            isFlying = false;
            return;
        }
        
        targetPoint = waypoints[startIndex];

        // Teleport to waypoint position
        transform.position = targetPoint.position;
        FaceTargetInstant();
        isFlying = true;

        // Skip movement this frame to prevent immediate drift
        skipNextUpdate = true;
    }

    /// <summary>
    /// 즉시 타겟을 바라보기
    /// </summary>
    private void FaceTargetInstant()
    {
        if (targetPoint == null)
            return;

        Vector3 dir = (targetPoint.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}
