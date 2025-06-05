using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WireTutorialUIBuilder : ITutorialUIBuilder
{
    private TutorialUIBase _tutorialUI;
    private Transform _parentCanvas;

    private VideoClip _wireExampleVideoClip;   // VideoClip
    private List<Sprite> _wireKeySprites;      // Wire key

    public WireTutorialUIBuilder(
        Transform parentCanvas,
        VideoClip wireExampleVideoClip,
        List<Sprite> wireKeySprites)
    {
        _parentCanvas = parentCanvas;
        _wireExampleVideoClip = wireExampleVideoClip;
        _wireKeySprites = wireKeySprites;
    }

    public void PrepareTutorialUI(Transform parentCanvas)
    {
        GameObject go = new GameObject("WireTutorialUI_Builder");
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
