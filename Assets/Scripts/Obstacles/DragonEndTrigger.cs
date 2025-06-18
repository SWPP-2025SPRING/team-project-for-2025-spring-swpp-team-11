using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class DragonEndTrigger : MonoBehaviour
{
    // 드래곤 등장 컷씬 때 일시정지 효과를 위해서 참조
    [SerializeField] private PlayableDirector dragonExitCutscene;
    [SerializeField] private CinemachineInputAxisController cameraMoveInputAxisController;
    [SerializeField] private ThrowingMonster throwingMonster;
    [SerializeField] private Rigidbody playerRb;

    private bool _dragonExited;
    
    private void DragonEnter()
    {
        // 플레이어 통제권 정지
        GameManager.Instance.InputManager.canControlPlayer = false;
        cameraMoveInputAxisController.enabled = false;
        
        playerRb.isKinematic = true;
        
        throwingMonster.gameObject.SetActive(true);
        throwingMonster.enabled = false;
        
        // 컷씬 재생
        dragonExitCutscene.Play();
        dragonExitCutscene.stopped += OnDragonEnterFinished;
    }

    private void OnDragonEnterFinished(PlayableDirector pd)
    {
        // 플레이어 통제권 복원
        Debug.Log("ASDG");
        
        GameManager.Instance.InputManager.canControlPlayer = true;
        cameraMoveInputAxisController.enabled = true;

        
        throwingMonster.gameObject.SetActive(false);

        playerRb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_dragonExited) return;
        if (other.CompareTag("Player"))
        {
            DragonEnter();
            _dragonExited = true;
        }
    }
}