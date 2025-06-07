using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public abstract class TutorialUIDecoratorBase : MonoBehaviour
{
    protected TutorialUIBase _innerUI;
    protected VideoClip _exampleClip;
    protected List<Sprite> _keySprites;
    protected string _explanationText;

    public void SetInnerUI(TutorialUIBase inner)
    {
        _innerUI = inner;
    }

    public virtual void Initialize(VideoClip clip, List<Sprite> keySprites, string explanation)
    {
        _exampleClip = clip;
        _keySprites = keySprites;
        _explanationText = explanation;
    }

    public virtual void Show()
    {
        if(_innerUI == null)
        {
            Debug.LogError($"[{this.GetType().Name}] inner UI가 설정되지 않았습니다!");
            return;
        }

        _innerUI.Show();

        // Explanation Text
        if(!string.IsNullOrEmpty(_explanationText))
        {
            _innerUI.SetExplanationText(_explanationText);
        }

        // Example Clip
        if(_exampleClip != null)
        {
            _innerUI.SetExampleVideo(_exampleClip);
        }

        // Key Icons
        // if (_keySprites != null)
        // {
        //     foreach (var key in _keySprites)
        //     {
        //         _innerUI.AddKeyIcon(key);
        //     }
        // }

        _innerUI.PlayVideo();
    }

    public virtual void Hide()
    {
        if(_innerUI != null)
        {
            _innerUI.Hide();
        }
    }

    public virtual void DestroyUI()
    {
        if(_innerUI != null)
        {
            _innerUI.DestroySelf();
        }
        Destroy(this.gameObject);
    }

    public TutorialUIBase GetInnerUI()
    {
        return _innerUI;
    }
    
}
