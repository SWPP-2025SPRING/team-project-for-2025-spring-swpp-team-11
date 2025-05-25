// PathObstaclePair.cs
using UnityEngine;

public class PathObstaclePair : MonoBehaviour
{
    [Header("Prefab & End")]
    public GameObject pillarPrefab;
    [HideInInspector] public Transform endPosition;

    [Header("Movement Settings")]
    public float moveSpeed   = 3f;   // 전진 속도 (z축)
    public float disZ        = 1.5f;   // 지그재그 전진 단위 (z축)
    public float lateralStep = 1f;   // 한 스텝당 좌우 이동량 (x축)
    public int   zigCount    = 4;    // 한 방향으로 몇 스텝 이동할지 (x축)
    public float disX        = 5f;   // 기둥 간격 (x축)

    private PathObstacleSpawnManager manager;
    private Vector3 spawnStart, pathDir, perpDir;
    private float halfDist;

    private float travelled;       // 누적 전진 거리
    private int   lastSegment;     // 마지막 처리한 구간
    private int   segmentsSinceToggle; // 한 방향으로 간 스텝 누적
    private float lateralOffset;   // 누적된 좌우 오프셋
    private bool  movingLeft;      // 현재 방향

    private GameObject leftPillar, rightPillar;

    public void Initialize(PathObstacleSpawnManager mgr)
    {
        manager             = mgr;
        travelled           = 0f;
        lastSegment         = -1;
        segmentsSinceToggle = 0;
        lateralOffset       = 0f;
        movingLeft          = true;

        spawnStart = mgr.transform.position;
        spawnStart.y = spawnStart.y;

        pathDir  = (endPosition.position - spawnStart).normalized;
        perpDir  = Vector3.Cross(pathDir, Vector3.up).normalized;
        halfDist = disX * 0.5f;

        // 두 기둥
        leftPillar  = Instantiate(pillarPrefab, transform);
        rightPillar = Instantiate(pillarPrefab, transform);
        leftPillar.transform.localPosition  = perpDir * halfDist;
        rightPillar.transform.localPosition = -perpDir * halfDist;
    }

    void Update()
    {
        // 1) 스폰 중이 아닐 땐 얼어붙음
        if (!manager.IsSpawning) return;

        // 2) 전진 거리 누적
        travelled += moveSpeed * Time.deltaTime;

        // 3) disZ 단위 구간 인덱스 계산
        int segment = Mathf.FloorToInt(travelled / disZ);

        // 4) 방향 toggle
        if (segment != lastSegment)
        {
            segmentsSinceToggle++;
            if (segmentsSinceToggle >= zigCount)
            {
                movingLeft = !movingLeft;
                segmentsSinceToggle = 0;
            }

            lateralOffset += (movingLeft ? -lateralStep : lateralStep);
            lastSegment = segment;
        }

        // 5) 중앙 위치 계산 및 이동
        Vector3 center = spawnStart 
                       + pathDir * travelled 
                       + perpDir * lateralOffset;
        center.y = spawnStart.y;
        transform.position = center;

        // 6) endPosition 넘어가면 이 쌍만 제거
        if (Vector3.Dot(endPosition.position - center, pathDir) < 0f)
            Destroy(gameObject);
    }
}
