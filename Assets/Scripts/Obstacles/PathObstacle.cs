// PathObstacle.cs
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PathObstacle : MonoBehaviour
{
    [Header("Hit Settings")]
    public float knockbackForce = 5f;
    public float stunDuration   = 1.5f;

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Destroy(gameObject);
    }
}
