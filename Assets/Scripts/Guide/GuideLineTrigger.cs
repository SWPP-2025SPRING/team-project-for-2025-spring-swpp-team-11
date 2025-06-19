using System.Collections.Generic;
using UnityEngine;

using GuidanceLine;


[RequireComponent(typeof(Collider))]
public class GuideLineTrigger : MonoBehaviour
{
    [Tooltip("Drag the GuidanceLine component here")]
    public GuidanceLine.GuidanceLine guidanceLine;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (guidanceLine != null) guidanceLine.MarkPassed(transform);
    }
}