using UnityEngine;

public class SandstormTriggerZone : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            SandstormSilhouetteFeature.IsActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            SandstormSilhouetteFeature.IsActive = false;
        }
    }
}
