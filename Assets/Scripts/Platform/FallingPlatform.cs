using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float collapseTime = 1.5f;
    public float respawnInterval = 3f;
    public float shakeIntensity = 1f;
    public float shakeFrequency = 400f;

    private Vector3 respawnPoint;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private float collapseTimer = 0f;
    private float respawnTimer = 0f;

    private bool isCollapsing = false;
    private bool isRespawning = false;

    private Rigidbody rb;
    private Collider col;

    private Vector3 shakeOffset = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        originalRotation = transform.rotation;
        respawnPoint = transform.position;
        originalScale = transform.localScale;

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        if (isCollapsing)
        {
            collapseTimer += Time.deltaTime;

            // Apply shaking effect
            float shake = Mathf.Sin(Time.time * shakeFrequency) * shakeIntensity;
            Vector3 offset = new Vector3(shake, 0f, shake);
            transform.position = respawnPoint + offset;

            if (collapseTimer >= collapseTime)
            {
                transform.position = respawnPoint; // reset before falling
                Fall();
            }
        }

        if (!rb.isKinematic && transform.position.y < -10f && !isRespawning)
        {
            isRespawning = true;
            respawnTimer = 0f;
        }

        if (isRespawning)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnInterval)
            {
                Respawn();
            }
        }
    }

    private void Fall()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        col.enabled = false;

        isCollapsing = false;
        collapseTimer = 0f;
    }

    private void Respawn()
    {
        foreach (Transform child in transform)
        {
            child.SetParent(null);
        }

        transform.position = respawnPoint;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;

        col.enabled = true;

        isRespawning = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCollapsing && rb.isKinematic)
        {
            isCollapsing = true;
            collapseTimer = 0f;
        }
    }
}
