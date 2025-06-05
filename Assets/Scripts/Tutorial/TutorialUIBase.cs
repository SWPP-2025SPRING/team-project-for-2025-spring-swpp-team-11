using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro; 

public class TutorialUIBase : MonoBehaviour
{
    // Prefab
    [Header("튜토리얼 패널 Prefab (ExampleVideo 포함)")]
    [SerializeField] private GameObject _panelPrefab;
    [Header("Key Icon Prefab")]
    [SerializeField] private GameObject _keyIconPrefab; 

    // Instance
    private GameObject _panelInstance;

    // Components
    private TextMeshProUGUI _explanationText;

    private VideoPlayer _exampleVideoPlayer;
    private RawImage    _exampleRawImage;

    private Transform _keyUIContainer;
    private Button    _nextButton;


    public void Initialize(Transform parentCanvas)
    {
        // Instantiate
        if (_panelPrefab == null)
        {
            Debug.LogError("[TutorialUIBase] _panelPrefab이 할당되지 않았습니다. Inspector에서 Prefab을 넣어 주세요.");
            return;
        }

        _panelInstance = Instantiate(_panelPrefab, parentCanvas, false);
        _panelInstance.name = "TutorialPanelInstance";

        // ExplanationText
        var explanationGO = _panelInstance.transform.Find("ExplanationText");
        if (explanationGO != null)
        {
            _explanationText = explanationGO.GetComponent<TextMeshProUGUI>();
            if (_explanationText == null)
                Debug.LogWarning("[TutorialUIBase] 'ExplanationText' 오브젝트에 TextMeshProUGUI 컴포넌트가 없습니다.");
        }
        else
        {
            Debug.LogWarning("[TutorialUIBase] 'ExplanationText' 오브젝트를 찾을 수 없습니다.");
        }

        // Video
        var videoGO = _panelInstance.transform.Find("ExampleVideo");
        if (videoGO != null)
        {
            _exampleVideoPlayer = videoGO.GetComponent<VideoPlayer>();
            if (_exampleVideoPlayer == null)
                Debug.LogWarning("[TutorialUIBase] 'ExampleVideo' 오브젝트에 VideoPlayer 컴포넌트가 없습니다.");

            _exampleRawImage = videoGO.GetComponentInChildren<RawImage>();
            if (_exampleRawImage == null)
                Debug.LogWarning("[TutorialUIBase] 'ExampleVideo' 하위에 RawImage 컴포넌트를 찾을 수 없습니다.");
        }
        else
        {
            Debug.LogWarning("[TutorialUIBase] 'ExampleVideo'라는 이름의 GameObject를 찾을 수 없습니다.");
        }

        // Key Container
        var keyContainerGO = _panelInstance.transform.Find("KeyUIContainer");
        if (keyContainerGO != null)
        {
            _keyUIContainer = keyContainerGO;
        }
        else
        {
            Debug.LogWarning("[TutorialUIBase] 'KeyUIContainer' 오브젝트를 찾을 수 없습니다.");
        }

        // Next Button
        var nextButtonGO = _panelInstance.transform.Find("NextButton");
        if (nextButtonGO != null)
        {
            _nextButton = nextButtonGO.GetComponent<Button>();
            if (_nextButton == null)
                Debug.LogWarning("[TutorialUIBase] 'NextButton' 오브젝트에 Button 컴포넌트가 없습니다.");
        }
        else
        {
            Debug.LogWarning("[TutorialUIBase] 'NextButton' 오브젝트를 찾을 수 없습니다.");
        }

        // Component 확인
        if (_explanationText == null ||
            (_exampleVideoPlayer == null || _exampleRawImage == null) ||
            _keyUIContainer == null ||
            _nextButton == null)
        {
            Debug.LogError("[TutorialUIBase] TutorialPanelPrefab 내부 자식 UI를 모두 찾지 못했습니다. 경로/계층 구조를 다시 확인하세요.");
        }

        if (_exampleRawImage != null)
            _exampleRawImage.gameObject.SetActive(false);
    }

    public void SetExplanationText(string message)
    {
        if (_explanationText != null)
        {
            _explanationText.text = message;
        }
    }

    public void SetExampleVideo(VideoClip clip)
    {
        if (_exampleVideoPlayer == null || _exampleRawImage == null)
        {
            Debug.LogWarning("[TutorialUIBase] VideoPlayer 또는 RawImage 컴포넌트를 찾을 수 없어 비디오를 재생할 수 없습니다.");
            return;
        }

        if (clip == null)
        {
            _exampleVideoPlayer.Stop();
            _exampleRawImage.gameObject.SetActive(false);
            return;
        }

        _exampleVideoPlayer.clip = clip;
        _exampleRawImage.gameObject.SetActive(true);

        _exampleVideoPlayer.Stop();
        _exampleVideoPlayer.Play();
    }

    public void AddKeyIcon(Sprite keySprite)
    {
        if (_keyUIContainer == null || _keyIconPrefab == null || keySprite == null)
            return;

        GameObject iconGO = Instantiate(_keyIconPrefab, _keyUIContainer);
        iconGO.name = $"KeyIcon_{keySprite.name}";

        Image img = iconGO.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = keySprite;
            img.preserveAspect = true;
        }
    }

    public void SetNextButtonCallback(Action onClick)
    {
        if (_nextButton == null)
            return;

        _nextButton.onClick.RemoveAllListeners();
        _nextButton.onClick.AddListener(() => onClick?.Invoke());
    }

    public void Show()
    {
        if (_panelInstance != null)
            _panelInstance.SetActive(true);
    }

    public void Hide()
    {
        if (_panelInstance != null)
            _panelInstance.SetActive(false);
    }

    public void DestroySelf()
    {
        if (_panelInstance != null)
            Destroy(_panelInstance);
        Destroy(this.gameObject);
    }
}
