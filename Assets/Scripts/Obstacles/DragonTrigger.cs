using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class DragonTrigger : MonoBehaviour
{
    // 드래곤 등장 컷씬 때 일시정지 효과를 위해서 참조
    [SerializeField] private PlayableDirector dragonEnterCutscene;
    [SerializeField] private CinemachineInputAxisController cameraMoveInputAxisController;
    [SerializeField] private ThrowingMonster throwingMonster;
    [SerializeField] private Rigidbody playerRb;

    private bool _dragonEntered;
    
    private void DragonEnter()
    {
        // 플레이어 통제권 정지
        GameManager.Instance.InputManager.canControlPlayer = false;
        cameraMoveInputAxisController.enabled = false;
        
        playerRb.isKinematic = true;
        
        throwingMonster.gameObject.SetActive(true);
        throwingMonster.enabled = false;
        
        // 컷씬 재생
        dragonEnterCutscene.Play();
        dragonEnterCutscene.stopped += OnDragonEnterFinished;
    }

    private void OnDragonEnterFinished(PlayableDirector pd)
    {
        // 플레이어 통제권 복원
        GameManager.Instance.InputManager.canControlPlayer = true;
        cameraMoveInputAxisController.enabled = true;

        playerRb.isKinematic = false;
        
        // 장애물 기믹 재생
        throwingMonster.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_dragonEntered) return;
        if (other.CompareTag("Player"))
        {
            DragonEnter();
            _dragonEntered = true;
        }
    }
}
