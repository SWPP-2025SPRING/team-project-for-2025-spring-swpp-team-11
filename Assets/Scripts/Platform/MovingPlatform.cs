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

    private bool _playerOnThis;

    private Transform _player;

    private void Start()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints set.");
            enabled = false;
            return;
        }

        _player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = waypoints[0].position;
        SetNextTarget();
    }

    private void FixedUpdate()
    {
        if (isWaiting)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                isWaiting = false;
                SetNextTarget();
            }
            return;
        }

        var current = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, currentTargetPoint, speed * Time.fixedDeltaTime);
        var diff = transform.position - current;

        if (Vector3.Distance(transform.position, currentTargetPoint) < 0.1f)
        {
            isWaiting = true;
        }

        if (_playerOnThis)
        {
            _player.Translate(diff, Space.World);
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
            _playerOnThis = true;
            // collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerOnThis = false;
            // collision.transform.SetParent(null);
        }
    }
}
