using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class JumpTutorialUIDecorator : TutorialUIDecoratorBase
{
    private const string DEFAULT_EXPLANATION = "Press Space to Jump. \n You can also press Space twice to double Jump.";

    public override void Show()
    {
        base.Show();
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
