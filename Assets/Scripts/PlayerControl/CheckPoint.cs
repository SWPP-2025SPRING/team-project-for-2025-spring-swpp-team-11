using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool _check = false;
    private void OnTriggerEnter(Collider other)
    {
        if (_check) return;
        if (other.tag == "Player")
        {
            Debug.Log("Check!");
            other.GetComponent<PlayerBehavior>().respawnPos = transform;
            _check = true;
        }
    }
}
