using UnityEngine;

public class FallingObstacleSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform spawnPoint;

    public Transform player;
    public float detectionRadius = 5f;
    public float detectionHeightOffset = 0.5f;
    public float interval = 1f;
    public float penalty = 5f;

    private float timer = 0f;
    private bool waiting = false;
    private float delay = 2f;

    void Start()
    {
        SpawnRock(); // Spawn one immediately at game start
    }

    void Update()
    {
        if (waiting)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                SpawnRock();
                waiting = false;
                timer = 0f;
            }
        }
    }

    public void TriggerRespawn(float respawnDelay)
    {
        delay = respawnDelay;
        waiting = true;
        timer = 0f;
    }

    private void SpawnRock()
    {
        GameObject obj = Instantiate(rockPrefab, spawnPoint.position, Quaternion.identity);

        BouncingRock rock = obj.GetComponent<BouncingRock>();
        if (rock != null)
        {
            rock.player = player;
            rock.detectionRadius = detectionRadius;
            rock.detectionHeightOffset = detectionHeightOffset;
            rock.interval = interval;
            rock.penalty = penalty;
            rock.spawner = this;
            rock.rb = obj.GetComponent<Rigidbody>();
        }
    }
}
