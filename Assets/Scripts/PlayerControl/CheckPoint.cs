using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Tooltip("이 지점에 리스폰되면 이 인덱스부터 가이드라인을 표시")]
    public int startCheckpointIndex = 0;

    [Tooltip("가이드라인 스크립트 참조")]
    public GuidanceLine.GuidanceLine guide;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerBehavior>().respawnPos = transform;
            guide.ResetGuidance(startCheckpointIndex);
        }
    }
}
