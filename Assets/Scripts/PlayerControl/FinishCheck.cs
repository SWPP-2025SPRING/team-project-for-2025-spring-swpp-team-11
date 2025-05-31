using System;
using UnityEngine;

public class FinishCheck : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && stageManager.currentStageState == StageState.Started)
        {
            stageManager.Finish();
            other.GetComponent<Rigidbody>().isKinematic = true;
            stageManager.currentStageState = StageState.Finished;
        }
    }
}
