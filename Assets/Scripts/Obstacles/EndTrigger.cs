// EndTrigger.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EndTrigger : MonoBehaviour
{
    [Tooltip("SpawnManager 레퍼런스")]
    public PathObstacleSpawnManager spawnManager;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1) 기둥 + end 충돌 시 spawn 중지
        if (other.GetComponentInParent<PathObstaclePair>() != null)
        {
            spawnManager.StopSpawning();
            return;
        }
        // 2) player + end 충돌 시 기둥 clear
        if (other.CompareTag("Player"))
        {
            spawnManager.ClearAll();
        }
    }
}
