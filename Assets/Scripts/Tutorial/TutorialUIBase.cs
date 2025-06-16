using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class TutorialUIBase : MonoBehaviour
{

    private TextMeshProUGUI _explanationText;
    private VideoPlayer     _exampleVideoPlayer;
    private RawImage        _exampleRawImage;
    private Transform       _keyUIContainer;
    private Button          _nextButton;

    private void Awake()
    {
        // 1) ExplanationText (TextMeshProUGUI)
        var explanationGO = transform.Find("ExplanationText");
        if (explanationGO != null)
            _explanationText = explanationGO.GetComponent<TextMeshProUGUI>();
        else
            Debug.LogWarning("[TutorialUIBase] ExplanationText 오브젝트를 찾을 수 없습니다.");

        // 2) ExampleVideo (VideoPlayer + RawImage)
        Transform videoGO = transform.Find("ExampleVideo");
        if (videoGO != null)
        {
            _exampleVideoPlayer = videoGO.GetComponent<VideoPlayer>();
             _exampleRawImage = videoGO.GetComponent<RawImage>();
        }
        else
        {
            Debug.LogWarning("[TutorialUIBase] ExampleVideo 오브젝트를 찾을 수 없습니다.");
        }

        // 3) KeyUIContainer
        var keyContainerGO = transform.Find("KeyUIContainer");
        if (keyContainerGO != null)
            _keyUIContainer = keyContainerGO;
        else
            Debug.LogWarning("[TutorialUIBase] KeyUIContainer 오브젝트를 찾을 수 없습니다.");

        // 4) NextButton (Button)
        var nextButtonGO = transform.Find("NextButton");
        if (nextButtonGO != null)
            _nextButton = nextButtonGO.GetComponent<Button>();
        else
            Debug.LogWarning("[TutorialUIBase] NextButton 오브젝트를 찾을 수 없습니다.");

        if (_exampleRawImage != null)
            _exampleRawImage.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }


    public void SetExplanationText(string message)
    {
        if (_explanationText != null)
            _explanationText.text = message;
    }

    public void SetExampleVideo(VideoClip clip)
    {
        if (clip == null)
        {
            _exampleVideoPlayer.Stop();
            _exampleRawImage.gameObject.SetActive(false);
            return;
        }

        _exampleRawImage.gameObject.SetActive(true);

        _exampleVideoPlayer.clip = clip;
        _exampleVideoPlayer.isLooping = true;
        _exampleVideoPlayer.Stop();
    }

    public void PlayVideo() => _exampleVideoPlayer.Play();

    public void AddKeyIcon(Sprite keySprite)
    {
        if (_keyUIContainer == null || keySprite == null)
            return;

        GameObject iconGO = new GameObject($"KeyIcon_{keySprite.name}", typeof(RectTransform), typeof(Image));
        iconGO.transform.SetParent(_keyUIContainer, false);

        var rt = iconGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);

        var img = iconGO.GetComponent<Image>();
        img.sprite = keySprite;
        img.preserveAspect = true;
    }

    public void SetNextButtonCallback(Action onClick)
    {
        if (_nextButton == null) return;
        _nextButton.onClick.RemoveAllListeners();
        _nextButton.onClick.AddListener(() => onClick?.Invoke());
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public RectTransform GetPanelRectTransform()
    {
        return GetComponent<RectTransform>();
    }
}
