using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

[System.Serializable]
public class KeyBindPair
{
    public string name;
    public TextMeshProUGUI text;

    public KeyBindPair(string name, TextMeshProUGUI text)
    {
        this.name = name;
        this.text = text;
    }
}

[System.Serializable]
public class SliderPair
{
    public string name;
    public Slider slider;

    public SliderPair(string name, Slider slider)
    {
        this.name = name;
        this.slider = slider;
    }
}

public class SetupUI : UIWindow
{
    public InputActionAsset actionAsset;
    public List<KeyBindPair> rebindTexts;
    public List<SliderPair> sliders;

    private Dictionary<string, TextMeshProUGUI> _rebindTexts = new Dictionary<string, TextMeshProUGUI>();
    private Dictionary<string, Slider> _sliders = new Dictionary<string, Slider>();
    private RebindingOperation _rebindingOperation;
    private InputAction _action;
    private string _rebindName;
    protected override void Start()
    {
        base.Start();
        foreach (KeyBindPair pair in rebindTexts)
        {
            _rebindTexts.Add(pair.name, pair.text);
            pair.text.SetText(actionAsset.FindActionMap("Player")
                                         .FindAction(pair.name)
                                         .GetBindingDisplayString(0));
        }
        foreach(SliderPair pair in sliders)
        {
            _sliders.Add(pair.name, pair.slider);
        }
        _sliders.GetValueOrDefault("sensitivity", null).value = Mathf.Log10(GameManager.Instance.DataManager.sensitivity);
        _sliders.GetValueOrDefault("BGM", null).value = GameManager.Instance.AudioManager.GetBGMVolume();
        _sliders.GetValueOrDefault("SFX", null).value = GameManager.Instance.AudioManager.GetSFXVolume();
    }

    public void StartRebind(string name)
    {
        var actionmap = actionAsset.FindActionMap("Player");
        _action = actionmap.FindAction(name);

        if(_action == null)
        {
            Debug.Log("Cannot find input action");
            return;
        }

        GameManager.Instance.AudioManager.PlayOneShot(SFX.HOVER_BUTTON);

        _rebindName = name;
        _action.Disable();
        _rebindingOperation = _action.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/rightButton")
            .WithCancelingThrough("<Mouse>/leftButton")
            .OnCancel(operation => RebindCancel())
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _rebindingOperation.Dispose();
        _action.Enable();

        string newKeyString = _action.GetBindingDisplayString(0);
        _rebindTexts.GetValueOrDefault(_rebindName, null).SetText(newKeyString);
        GameManager.Instance.AudioManager.PlayOneShot(SFX.CLICK_BUTTON);
    }

    private void RebindCancel()
    {
        GameManager.Instance.AudioManager.PlayOneShot(SFX.HOVER_BUTTON);
        _rebindingOperation.Dispose();
        _action.Enable();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateSensitivity()
    {
        float logSensitivity = _sliders.GetValueOrDefault("sensitivity", null).value;
        Debug.Log(logSensitivity);
        float sensitivity = Mathf.Pow(10, logSensitivity);
        GameManager.Instance.DataManager.SetSensitivity(sensitivity);
    }

    public void UpdateBGMVolume()
    {
        float volume = _sliders.GetValueOrDefault("BGM", null).value;
        GameManager.Instance.AudioManager.SetBGMVolume(volume);
    }

    public void UpdateSFXVolume()
    {
        float volume = _sliders.GetValueOrDefault("SFX", null).value;
        GameManager.Instance.AudioManager.SetSFXVolume(volume);
    }

    public void GotoTitle()
    {
        GameManager.Instance.SceneLoadManager.FadeLoadScene("0_TitleSceneAppliedOne");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
