using UnityEngine;

public class CannonProjectileSpawner : MonoBehaviour, ICannonObserver
{
    public Cannon cannon;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public Vector3 launchDirection = Vector3.left;
    public float speed = 10f;

    private void Start()
    {
        cannon.RegisterObserver(this);
    }

    public void OnCannonFired()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.linearVelocity = launchDirection.normalized * speed;
    }
}
