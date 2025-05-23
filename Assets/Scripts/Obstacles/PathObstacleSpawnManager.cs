using System.Collections;
using UnityEngine;

public class PathObstacleSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject obstaclePrefab;
    public Transform startPosition;
    public Transform player;

    [Space]
    public float interval = 1f;    // 생성 간격
    public float disZ = 2f;        // 전진 간격
    public float disX = 5f;        // 기둥 (좌우) 간격
    public float lateralStep = 1f; // 한 스텝당 좌우 이동 거리
    public int zigCount = 5;       // 한쪽으로 연속 생성 개수
    public float speed = 10f;      // 상승 속도

    private Coroutine spawnRoutine;
    private Vector3 pathDir;
    private Vector3 perpDir;
    private Vector3 currentBasePos;

    void Start()
    {
        pathDir        = (transform.position - startPosition.position).normalized;
        perpDir        = Vector3.Cross(pathDir, Vector3.up).normalized;
        currentBasePos = startPosition.position;
    }

    void Update()
    {
        float playerDistSqr = (startPosition.position - player.position).sqrMagnitude;
        float totalDistSqr  = (startPosition.position - transform.position).sqrMagnitude;
        // 스포너를 향한 벡터 (수평면)
        Vector3 toSpawner = transform.position - player.position;
        toSpawner.y = 0;

        // player가 spawner 바라보는지
        bool isFacingSpawner = Vector3.Dot(player.forward, toSpawner.normalized) > 0f;

        // 스폰 시작 : 거리 & spawner 바라보는지
        if (spawnRoutine == null && playerDistSqr < totalDistSqr && isFacingSpawner)
            spawnRoutine = StartCoroutine(SpawnLoop());
        // 스폰 종료
        else if (spawnRoutine != null && playerDistSqr >= totalDistSqr)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
            ClearRemainingObstacles();
            currentBasePos = startPosition.position;
        }
    }

    private IEnumerator SpawnLoop()
    {
        int stepIndex = 0;
        int cycleLen  = zigCount * 2;
        float halfDist = disX * 0.5f;

        while (true)
        {
            // 1) 전진 기준 (이전 중앙 위치에서)
            Vector3 forwardPos = currentBasePos + pathDir * disZ;
            forwardPos.y = startPosition.position.y;

            // 2) 지그재그 인덱스 계산
            int cycleIndex = stepIndex % cycleLen;

            // 3) 중앙 오프셋 계산 (왼쪽/오른쪽)
            float centerOffset = (cycleIndex < zigCount)
                ? -(cycleIndex + 1) * lateralStep
                :  (cycleIndex - zigCount + 1) * lateralStep;

            // 4) 중앙 위치
            Vector3 centerPos = forwardPos + perpDir * centerOffset;
            centerPos.y = forwardPos.y;

            // 5) 중앙 기준 ±halfDist 에 두 기둥 생성
            Vector3 spawnPos1 = centerPos + perpDir * halfDist;
            Vector3 spawnPos2 = centerPos - perpDir * halfDist;
            spawnPos1.y = spawnPos2.y = forwardPos.y;

            Instantiate(obstaclePrefab, spawnPos1, Quaternion.identity)
                .GetComponent<PathObstacle>().riseSpeed = speed;
            Instantiate(obstaclePrefab, spawnPos2, Quaternion.identity)
                .GetComponent<PathObstacle>().riseSpeed = speed;

            stepIndex++;
            currentBasePos = centerPos;

            // 6) 끝점 체크
            if ((startPosition.position - currentBasePos).sqrMagnitude
                >= (startPosition.position - transform.position).sqrMagnitude)
                break;

            yield return new WaitForSeconds(interval);
        }
    }



    private void ClearRemainingObstacles()
    {
        foreach (var o in FindObjectsOfType<PathObstacle>())
            Destroy(o.gameObject);
    }
}
