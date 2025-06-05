using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class JumpTutorialUIBuilder : ITutorialUIBuilder
{
    private TutorialUIBase _tutorialUI;
    private Transform _parentCanvas;

    private VideoClip _jumpExampleVideoClip;   // VideoClip
    private List<Sprite> _jumpKeySprites;      // Jump key

    public JumpTutorialUIBuilder(
        Transform parentCanvas,
        VideoClip jumpExampleVideoClip,
        List<Sprite> jumpKeySprites)
    {
        _parentCanvas = parentCanvas;
        _jumpExampleVideoClip = jumpExampleVideoClip;
        _jumpKeySprites = jumpKeySprites;
    }

    public void PrepareTutorialUI(Transform parentCanvas)
    {
        GameObject go = new GameObject("JumpTutorialUI_Builder");
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
