using UnityEngine;

public class UpwardObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject upwardObstaclePrefab;
    public Transform spawnPoint;

    [Header("Shared Parameters")]
    public Transform player;
    public float detectionRadius = 3f;
    public float detectionHeightOffset = 0.5f;
    public float interval = 1f;
    public float penalty = 5f;

    private float timer = 0f;
    private bool waitingToRespawn = false;
    private float delay = 2f;

    private void Start()
    {
        SpawnObstacle(); // 처음 1회 생성
    }

    private void Update()
    {
        if (waitingToRespawn)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                SpawnObstacle();
                timer = 0f;
                waitingToRespawn = false;
            }
        }
    }

    public void TriggerRespawn(float respawnDelay)
    {
        delay = respawnDelay;
        waitingToRespawn = true;
        timer = 0f;
    }

    private void SpawnObstacle()
    {
        GameObject obj = Instantiate(upwardObstaclePrefab, spawnPoint.position, Quaternion.identity);
        UpwardObstacle obstacle = obj.GetComponent<UpwardObstacle>();
        if (obstacle != null)
        {
            obstacle.player = player;
            obstacle.detectionRadius = detectionRadius;
            obstacle.detectionHeightOffset = detectionHeightOffset;
            obstacle.interval = interval;
            obstacle.penalty = penalty;
            obstacle.spawner = this;
        }
    }
}
