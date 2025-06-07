using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public interface ITutorialUIBuilder
{
    void PrepareTutorialUI(Transform parentCanvas);
    void BuildExplanation(string message);
    void BuildExampleVideo(VideoClip clip);
    void BuildKeyUI(List<Sprite> keySprites);
    void BuildNextButton(Action onClickCallback);

    TutorialUIBase GetResult();
}
