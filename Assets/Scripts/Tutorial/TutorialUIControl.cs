using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class TutorialUIControl : MonoBehaviour
{
    [SerializeField] private bool _enabled = false; 
    [SerializeField] private int _targetStageIndex = 1;

    [SerializeField] public Transform _playerTransform;
    private StageManager _stageManager;

    [SerializeField] private VideoClip    _moveExampleClip;
    [SerializeField] private List<Sprite> _moveKeySprites;
    private bool _moveTriggered = false;

    [SerializeField] private VideoClip    _jumpExampleClip;
    [SerializeField] private List<Sprite> _jumpKeySprites;
    private bool _jumpTriggered = false;

    [SerializeField] private VideoClip    _wireExampleClip;
    [SerializeField] private List<Sprite> _wireKeySprites;
    private bool _wireTriggered = false;

    [SerializeField] private Canvas _targetCanvas;
    
    [SerializeField] private GameObject _tutorialUIPrefab;

    private TutorialDirector        _tutorialDirector;
    private TutorialUIDecoratorBase _currentDecorator;

    private void Awake()
    {

        _stageManager = FindObjectOfType<StageManager>();
        _tutorialDirector = new TutorialDirector(_tutorialUIPrefab);

        if (_targetCanvas == null)
        {
            _targetCanvas = FindObjectOfType<Canvas>();
        }
    }

    private void Update()
    {
        if (!_enabled) 
            return;

        if (_stageManager == null || _stageManager.stageNumber != _targetStageIndex)
            return;

        if (_currentDecorator != null)
            return;

    }

    public void TriggerTutorial(TutorialType type)
    {
        if (!_enabled || _stageManager.stageNumber != _targetStageIndex)
            return;

        switch (type)
        {
            case TutorialType.Move:
                if (_moveTriggered) return;
                _moveTriggered = true;
                ShowTutorial(type, _moveExampleClip, _moveKeySprites, Vector2.zero);
                break;

            case TutorialType.Jump:
                if (_jumpTriggered) return;
                _jumpTriggered = true;
                ShowTutorial(type, _jumpExampleClip, _jumpKeySprites, new Vector2(-100f, 50f));
                break;

            case TutorialType.Wire:
                if (_wireTriggered) return;
                _wireTriggered = true;
                ShowTutorial(type, _wireExampleClip, _wireKeySprites, new Vector2(100f, -30f));
                break;
        }
    }

    private void ShowTutorial(
        TutorialType type,
        VideoClip exampleClip,
        List<Sprite> keySprites,
        Vector2 uiPosition
    )
    {
        // Pause
        Time.timeScale   = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // Canvas
        Transform canvasT = _targetCanvas != null
            ? _targetCanvas.transform
            : (FindObjectOfType<Canvas>()?.transform);

        if (canvasT == null)
        {
            Debug.LogError("[TutorialUIControl] Canvas가 없습니다. 튜토리얼 UI를 띄울 수 없습니다.");
            return;
        }

        // Director
        switch (type)
        {
            case TutorialType.Move:
                _currentDecorator = _tutorialDirector.ConstructMoveTutorial(
                    canvasT,
                    exampleClip,
                    keySprites,
                    OnTutorialNext
                );
                break;

            case TutorialType.Jump:
                _currentDecorator = _tutorialDirector.ConstructJumpTutorial(
                    canvasT,
                    exampleClip,
                    keySprites,
                    OnTutorialNext
                );
                break;

            case TutorialType.Wire:
                _currentDecorator = _tutorialDirector.ConstructWireTutorial(
                    canvasT,
                    exampleClip,
                    keySprites,
                    OnTutorialNext
                );
                break;
        }

        if (_currentDecorator == null)
            return;

        RectTransform panelRT = _currentDecorator
            .GetInnerUI()
            .GetPanelRectTransform();

        if (panelRT != null)
            panelRT.anchoredPosition = uiPosition;

        _currentDecorator.Show();
    }

    private void OnTutorialNext()
    {
        if (_currentDecorator != null)
        {
            _currentDecorator.Hide();
            _currentDecorator.DestroyUI();
            _currentDecorator = null;
        }

        // Resume
        Time.timeScale   = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    public void EnableTutorial()
    {
        _enabled = true;
    }

    public enum TutorialType { Move, Jump, Wire }
}
