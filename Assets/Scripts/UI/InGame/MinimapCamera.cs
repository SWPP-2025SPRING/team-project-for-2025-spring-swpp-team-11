using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public PlayerBehavior player;
    public GameObject cam;
    public Vector3 offset = new Vector3(0, 200, 0);
    void Start()
    {
        
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
        transform.rotation = Quaternion.Euler(90, cam.transform.rotation.eulerAngles.y, 0);
    }
}
