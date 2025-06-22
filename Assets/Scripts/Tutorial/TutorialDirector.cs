using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class TutorialDirector
{
    private GameObject _tutorialUIPrefab;

    public TutorialDirector(GameObject tutorialUIPrefab)
    {
        _tutorialUIPrefab = tutorialUIPrefab;
    }

    public TutorialUIDecoratorBase ConstructMoveTutorial(
        Transform parentCanvas,
        VideoClip exampleClip,
        List<Sprite> keySprites,
        Action onNextCallback
    )
    {
        var builder = new MoveTutorialUIBuilder(parentCanvas, _tutorialUIPrefab);
        builder.PrepareTutorialUI(parentCanvas);
        builder.BuildExplanation("Use WASD key to move.");
        builder.BuildExampleVideo(exampleClip);
        builder.BuildKeyUI(keySprites);
        builder.BuildNextButton(onNextCallback);
        var uiBase = builder.GetResult();
        
        GameObject decoratorGO = new GameObject("MoveTutorialDecorator");
        var decorator = decoratorGO.AddComponent<MoveTutorialUIDecorator>();
        decoratorGO.transform.SetParent(parentCanvas, false);
        decorator.SetInnerUI(uiBase);
        decorator.Initialize(exampleClip, keySprites, "Use WASD key to move.");

        return decorator;
    }

    public TutorialUIDecoratorBase ConstructJumpTutorial(
        Transform parentCanvas,
        VideoClip exampleClip,
        List<Sprite> keySprites,
        Action onNextCallback
    )
    {
        var builder = new JumpTutorialUIBuilder(parentCanvas, _tutorialUIPrefab);
        builder.PrepareTutorialUI(parentCanvas);
        builder.BuildExplanation("Press Space to Jump. \n You can also press Space twice to double Jump.");
        builder.BuildExampleVideo(exampleClip);
        builder.BuildKeyUI(keySprites);
        builder.BuildNextButton(onNextCallback);
        var uiBase = builder.GetResult();

        GameObject decoratorGO = new GameObject("JumpTutorialDecorator");
        var decorator = decoratorGO.AddComponent<JumpTutorialUIDecorator>();
        decoratorGO.transform.SetParent(parentCanvas, false);
        decorator.SetInnerUI(uiBase);
        decorator.Initialize(exampleClip, keySprites, "Press Space to Jump. \n You can also press Space twice to double Jump.");
        return decorator;
    }

    public TutorialUIDecoratorBase ConstructWireTutorial(
        Transform parentCanvas,
        VideoClip exampleClip,
        List<Sprite> keySprites,
        Action onNextCallback
    )
    {
        var builder = new WireTutorialUIBuilder(parentCanvas, _tutorialUIPrefab);
        builder.PrepareTutorialUI(parentCanvas);
        builder.BuildExplanation("Click to initiate wire.\n Use WASD to swing while attached to the wire.");
        builder.BuildExampleVideo(exampleClip);
        builder.BuildKeyUI(keySprites);
        builder.BuildNextButton(onNextCallback);
        var uiBase = builder.GetResult();
        
        GameObject decoratorGO = new GameObject("WireTutorialDecorator");
        var decorator = decoratorGO.AddComponent<WireTutorialUIDecorator>();
        decoratorGO.transform.SetParent(parentCanvas, false);
        decorator.SetInnerUI(uiBase);
        decorator.Initialize(exampleClip, keySprites, "Click to initiate wire.\n Use WASD to swing while attached to the wire.");
        return decorator;
    }
}
