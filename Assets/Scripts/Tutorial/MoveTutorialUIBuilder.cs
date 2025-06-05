using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MoveTutorialUIBuilder : ITutorialUIBuilder
{
    private TutorialUIBase _tutorialUI;
    private Transform _parentCanvas;

    private VideoClip _moveExampleVideoClip;   // VideoClip
    private List<Sprite> _moveKeySprites;      // W, A, S, D key icon

    public MoveTutorialUIBuilder(
        Transform parentCanvas,
        VideoClip moveExampleVideoClip,
        List<Sprite> moveKeySprites)
    {
        _parentCanvas = parentCanvas;
        _moveExampleVideoClip = moveExampleVideoClip;
        _moveKeySprites = moveKeySprites;
    }

    public void PrepareTutorialUI(Transform parentCanvas)
    {
        GameObject go = new GameObject("MoveTutorialUI_Builder");
        go.transform.SetParent(parentCanvas, false);

        _tutorialUI = go.AddComponent<TutorialUIBase>();
        _tutorialUI.Initialize(parentCanvas);
    }

    public void BuildExplanation(string message)
    {
        _tutorialUI.SetExplanationText(message);
    }

    public void BuildExampleVideo(VideoClip exampleClip)
    {
        _tutorialUI.SetExampleVideo(exampleClip);
    }

    public void BuildKeyUI(List<Sprite> keySprites)
    {
        foreach (var keySprite in keySprites)
        {
            _tutorialUI.AddKeyIcon(keySprite);
        }
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
