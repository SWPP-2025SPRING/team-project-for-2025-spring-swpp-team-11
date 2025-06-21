using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GuideBirdPoint : MonoBehaviour
{
    [Tooltip("씬에 떠 있는 GuideBird 오브젝트")]
    public GameObject guideBirdInstance;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (guideBirdInstance == null)
        {
            Debug.LogWarning("GuideBirdInstance가 할당되지 않았습니다.");
            return;
        }

        // GuideBirdController를 가져와서 다음 지점으로 이동 호출
        var ctrl = guideBirdInstance.GetComponent<GuideBirdController>();
        if (ctrl != null)
        {
            ctrl.GoToNextPoint();
        }
    }
}
