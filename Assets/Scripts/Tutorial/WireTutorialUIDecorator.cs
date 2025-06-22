using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WireTutorialUIDecorator : TutorialUIDecoratorBase
{
    private const string DEFAULT_EXPLANATION = "Click to initiate wire.\n Use WASD to swing while attached to the wire.";

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
