using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GuideBirdDestroyTrigger : MonoBehaviour
{
    public string guideBirdTag = "GuideBird";

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var guide = GameObject.FindWithTag(guideBirdTag);
        if (guide != null)
            Destroy(guide);
    }
}
