using UnityEngine;

public class GuideBirdController : MonoBehaviour
{
    [Tooltip("플레이어 Transform (혹은 태그로 찾기)")]
    public Transform player;

    [Tooltip("이 거리 이상 멀어지면 GuideBird를 비활성화/제거")]
    public float disappearDistance = 50f;

    [Tooltip("사라질 때 Destroy 할지, 단순히 비활성화할지")]
    public bool destroyOnDisappear = true;

    void Start()
    {
        // Inspector에 할당이 안 돼 있으면 Tag로 찾아오기
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) player = go.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > disappearDistance)
        {
            if (destroyOnDisappear)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
