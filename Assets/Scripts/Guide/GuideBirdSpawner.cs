using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GuideBirdSpawner : MonoBehaviour
{
    [Tooltip("Player가 이 지점을 통과하면 GuideBird가 스폰될 위치")]
    public Transform birdSpawnLocation;

    [Tooltip("스폰할 GuideBird Prefab")]
    public GameObject guideBirdPrefab;

    [Tooltip("한 번만 작동시키고 싶으면 체크")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void Reset()
    {
        // Collider -> Trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

         // Spawn Guidebird
        GameObject guide = Instantiate(
            guideBirdPrefab,
            birdSpawnLocation.position,
            birdSpawnLocation.rotation
        );

        // Guidebird -> Player 바라보도록
        Vector3 lookDir = other.transform.position - guide.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            guide.transform.rotation = Quaternion.LookRotation(lookDir);

        if (triggerOnce)
            hasTriggered = true;
    }
}
