using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MoveTutorialUIDecorator: TutorialUIDecoratorBase
{
    private const string DEFAULT_EXPLANATION = "Use WASD key to move.";

    public override void Show()
    {
        base.Show();
        // Only in Move
    }

    public override void Initialize(VideoClip clip, List<Sprite> keySprites, string explanation)
    {
        if (string.IsNullOrEmpty(explanation))
            _explanationText = DEFAULT_EXPLANATION;
        else
            _explanationText = explanation;

        _exampleClip = clip;
        _keySprites = keySprites;
    }

}