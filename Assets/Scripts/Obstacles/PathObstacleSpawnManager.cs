using System.Collections;
using UnityEngine;

public class PathObstacleSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject obstaclePrefab;
    public Transform startPosition;
    public Transform player;

    [Space]
    public float interval = 0.2f;    // 생성 간격
    public float disZ = 1.5f;        // 전진 간격
    public float disX = 5f;        // 기둥 (좌우) 간격
    public float lateralStep = 0.5f; // 한 스텝당 좌우 이동 거리
    public int zigCount = 5;       // 한쪽으로 연속 생성 개수
    public float speed = 10f;      // 상승 속도

    private Coroutine spawnRoutine;
    private Vector3 pathDir;
    private Vector3 perpDir;
    private Vector3 currentBasePos;

    private Vector3 lastPlayerPos;
    public float lateralThreshold = 3f; // 스폰 시작 조건

    void Start()
    {
        pathDir        = (transform.position - startPosition.position).normalized;
        perpDir        = Vector3.Cross(pathDir, Vector3.up).normalized;
        currentBasePos = startPosition.position;
        lastPlayerPos = player.position;
    }

    void Update()
    {
        // 1) start→spawner 방향
        Vector3 playerPathVec = (transform.position - startPosition.position);
        Vector3 playerPathDir = playerPathVec.normalized;
        float pathLenSqr = playerPathVec.sqrMagnitude;
        
        if (pathLenSqr < Mathf.Epsilon) return;

        // 2) 파라미터 t 계산: 0=start, 1=spawner
        Vector3 AP = player.position - startPosition.position;
        float t = Vector3.Dot(AP, playerPathVec) / pathLenSqr;

        // 3) 실제 이동 방향 (프레임간 위치 변화)
        Vector3 moveDir = player.position - lastPlayerPos;
        moveDir.y = 0;
        bool isMovingToward = moveDir.sqrMagnitude > 0f 
            && Vector3.Dot(moveDir.normalized, playerPathDir) > 0f;

        // 4) 좌우 간격 확인
        float lateralOffset = player.position.x - startPosition.position.x;
        bool withinLateral = Mathf.Abs(lateralOffset) <= lateralThreshold;

        // 5) 스폰 시작/종료 판정
        if (spawnRoutine == null)
        {
            // t∈(0,1) 구간에 있고, spawner 방향으로 이동 중일 때만 시작
            if (t > 0f && t < 1f && isMovingToward && withinLateral)
                spawnRoutine = StartCoroutine(SpawnLoop());
        }
        else
        {
            // t 벗어나거나 반대 방향 이동 시 즉시 종료
            if (!(t > 0f && t < 1f))
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
                ClearRemainingObstacles();
                currentBasePos = startPosition.position;
            }
        }
        lastPlayerPos = player.position;
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