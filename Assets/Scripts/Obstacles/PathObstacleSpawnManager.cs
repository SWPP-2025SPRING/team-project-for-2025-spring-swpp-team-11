// PathObstacleSpawnManager.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PathObstacleSpawnManager : MonoBehaviour
{
    [Header("Pair Prefab & End")]
    public PathObstaclePair pairPrefab;
    public Transform        endPosition;

    [Header("Spawn Settings")]
    public float spawnInterval = 0.5f;

    [HideInInspector] public bool IsSpawning { get; private set; }

    private Coroutine spawnRoutine;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")
            || spawnRoutine != null
            || FindObjectsOfType<PathObstaclePair>().Length > 0)
        {
            return;
        }

        IsSpawning   = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (!other.CompareTag("Player")) return;
    //     StopSpawning();
    // }

    public void StopSpawning()
    {
        if (!IsSpawning) return;
        IsSpawning = false;
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    public void ClearAll()
    {
        foreach (var pair in FindObjectsOfType<PathObstaclePair>())
            Destroy(pair.gameObject);
    }

    private IEnumerator SpawnLoop()
    {
        while (IsSpawning)
        {
            var pair = Instantiate(pairPrefab, transform.position, Quaternion.identity);
            pair.endPosition = endPosition;
            pair.Initialize(this);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
