using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PathObstacle : MonoBehaviour
{
    public float riseSpeed = 10f;        
    private float groundY;

    void Start()
    {
        groundY = 0f;
    }

    void Update()
    {
        Vector3 target = new Vector3(transform.position.x, groundY, transform.position.z);
        if (transform.position.y < groundY)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, riseSpeed * Time.deltaTime);
        }
    }

}
