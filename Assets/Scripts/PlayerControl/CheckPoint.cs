using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Tooltip("이 지점에 리스폰되면 이 인덱스부터 가이드라인을 표시")]
    public int startCheckpointIndex = 0;
    public int birdCheckpointIndex = 0;

    [Tooltip("가이드라인 스크립트 참조")]
    public GuidanceLine.GuidanceLine guide;

    [Tooltip("씬에 떠 있는 GuideBirdController 인스턴스를 할당")]
    public GuideBirdController birdController;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 1) 플레이어 리스폰 위치 저장
        var pb = other.GetComponent<PlayerBehavior>();
        if (pb != null)
            pb.respawnPos = transform;

        // 2) 가이드 라인 리셋
        if (guide != null)
            guide.ResetGuidance(startCheckpointIndex);

        // 3) 가이드 버드 리셋
        if (birdController != null)
            birdController.ResetGuidance(birdCheckpointIndex);
    }
}
