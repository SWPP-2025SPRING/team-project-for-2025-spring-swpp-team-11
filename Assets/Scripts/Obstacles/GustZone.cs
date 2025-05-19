using UnityEngine;

public class GustZone : MonoBehaviour
{
    [Header("Gust Settings")]
    public Vector3 gustDirection = Vector3.forward; // 밀리는 방향
    public float gustForce = 10f;

    [Header("Target Settings")]
    public string playerTag = "Player";

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 방향 정규화 후 ForceMode.Force로 지속적 적용
                rb.AddForce(gustDirection.normalized * gustForce, ForceMode.Force);
            }
        }
    }

}
