using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialUIControl _tutorialControl;
    [SerializeField] private TutorialUIControl.TutorialType _type;

    private bool _hasTriggered = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"{name} 오브젝트의 Collider는 isTrigger로 설정되어 있지 않습니다. 자동으로 활성화합니다.");
            col.isTrigger = true;
        }

        if (_tutorialControl == null)
            Debug.LogError($"[{name}] TutorialUIControl이 할당되지 않았습니다.");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Tutorial은 한번만!
        if (_hasTriggered || _tutorialControl == null)
            return;

        // Player와의 충돌인지 확인    
        if (other.transform == _tutorialControl._playerTransform)
        {
            _hasTriggered = true;
            _tutorialControl.TriggerTutorial(_type);
        }
    }
}
