using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MoveTutorialUIBuilder : ITutorialUIBuilder
{
    private TutorialUIBase _tutorialUI;
    private Transform      _parentCanvas;
    private GameObject _tutorialUIPrefab;

    public MoveTutorialUIBuilder(Transform parentCanvas, GameObject tutorialUIPrefab)
    {
        _parentCanvas     = parentCanvas;
        _tutorialUIPrefab = tutorialUIPrefab;
    }

    public void PrepareTutorialUI(Transform parentCanvas)
    {
        if (_tutorialUIPrefab == null)
        {
            Debug.LogError("[MoveTutorialUIBuilder] TutorialUI.prefab 레퍼런스가 없습니다.");
            return;
        }

        GameObject containerGO = GameObject.Instantiate(
            _tutorialUIPrefab, 
            parentCanvas, 
            false
        );
        containerGO.name = "MoveTutorialUI_Instance";

        // TutorialUIBase
        _tutorialUI = containerGO.GetComponent<TutorialUIBase>();
        if (_tutorialUI == null)
        {
            Debug.LogError("[MoveTutorialUIBuilder] TutorialUIBase 컴포넌트를 찾을 수 없습니다.");
            return;
        }

    }

    public void BuildExplanation(string message)
    {
        _tutorialUI.SetExplanationText(message);
    }

    public void BuildExampleVideo(VideoClip clip)
    {
        _tutorialUI.SetExampleVideo(clip);
    }

    public void BuildKeyUI(List<Sprite> keySprites)
    {
        foreach (var keySprite in keySprites)
            _tutorialUI.AddKeyIcon(keySprite);
    }

    public void BuildNextButton(Action onClickCallback)
    {
        _tutorialUI.SetNextButtonCallback(onClickCallback);
    }

    public TutorialUIBase GetResult()
    {
        return _tutorialUI;
    }
}
