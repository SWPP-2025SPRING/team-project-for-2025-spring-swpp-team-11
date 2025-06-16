using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    public List<Transform> waypoints; // ← Transform으로 변경
    public float speed = 2f;
    public float interval = 2f;

    private int currentIndex = 0;
    private Vector3 currentTargetPoint;
    private float timer = 0f;
    private bool isWaiting = false;

    private void Start()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints set.");
            enabled = false;
            return;
        }

        transform.position = waypoints[0].position;
        SetNextTarget();
    }

    private void FixedUpdate()
    {
        if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                isWaiting = false;
                SetNextTarget();
            }
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTargetPoint, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTargetPoint) < 0.1f)
        {
            isWaiting = true;
        }
    }

    private void SetNextTarget()
    {
        currentIndex = (currentIndex + 1) % waypoints.Count;
        currentTargetPoint = waypoints[currentIndex].position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
