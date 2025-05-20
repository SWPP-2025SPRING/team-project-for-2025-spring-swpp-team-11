using System.Collections;
using UnityEngine;

public class PathObstacleSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject obstaclePrefab;
    public Transform startPosition;
    public Transform endPosition;
    public Transform player;

    [Space]
    public float interval = 1f;    // 생성 간격
    public float disZ = 2f;        // 전진 오프셋 (경로 방향)
    public float maxX = 7f;        // 좌/우 최대 거리 (unused here)
    public float disX = 2f;        // 기둥 한 쌍 간 거리
    public float speed = 10f;

    private Coroutine spawnRoutine;
    private Vector3 pathDir;       
    private Vector3 perpDir;       
    private Vector3 currentBasePos; // 이번 스폰의 기준점

    void Start()
    {
        pathDir = (endPosition.position - startPosition.position).normalized;
        perpDir = Vector3.Cross(pathDir, Vector3.up).normalized;
        currentBasePos = startPosition.position; // 초기 기준점
    }

    void Update()
    {
        float playerDist = (startPosition.position - player.position).sqrMagnitude;
        float totalDist  = (startPosition.position - endPosition.position).sqrMagnitude;

        if (spawnRoutine == null && playerDist < totalDist)
            spawnRoutine = StartCoroutine(SpawnLoop());
        else if (spawnRoutine != null && playerDist >= totalDist)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
            ClearRemainingObstacles();
        }
    }

    private IEnumerator SpawnLoop()
    {
        float halfDist = disX * 0.5f;

        while (true)
        {
            // 1) 이번 스폰의 기준점에서 경로 방향으로 disZ 전진
            Vector3 forwardPos = currentBasePos + pathDir * disZ;

            // 2) perpDir으로 랜덤하게 살짝 shift
            float lateralShift = Random.Range(-maxX, maxX);
            Vector3 centerPos = forwardPos + perpDir * lateralShift;
            centerPos.y = startPosition.position.y;
            
            // 3) 중앙 위치 기준 ±halfDist 에 기둥 스폰
            for (int sign = -1; sign <= 1; sign += 2)
            {
                Vector3 spawnPos = centerPos + perpDir * (halfDist * sign);
                spawnPos.y = startPosition.position.y;

                var obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
                var obs = obj.GetComponent<PathObstacle>();
                if (obs != null) obs.riseSpeed = speed;
            }

            // 4) 다음 스폰의 기준점은 바로 이 centerPos
            currentBasePos = centerPos;

            float currentBaseDist = (startPosition.position - currentBasePos).sqrMagnitude;
            float totalDist  = (startPosition.position - endPosition.position).sqrMagnitude;

            if (currentBaseDist > totalDist)
            {
                break;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private void ClearRemainingObstacles()
    {
        foreach (var o in FindObjectsOfType<PathObstacle>())
            Destroy(o.gameObject);
        
        // 기준점도 리셋
        currentBasePos = startPosition.position;
    }
}
