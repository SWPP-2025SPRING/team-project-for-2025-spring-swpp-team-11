using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WireSetupButton : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _button.onClick.AddListener(ToggleWireMode);
        ApplyWireModeText();
    }

    private void ToggleWireMode()
    {
        var dm = GameManager.Instance.DataManager;
        if(dm.wiremode == WIREMODE.HOLD)
        {
            dm.SetWireMode(WIREMODE.TOGGLE);
        }
        else if(dm.wiremode == WIREMODE.TOGGLE)
        {
            dm.SetWireMode(WIREMODE.HOLD);
        }
        EventSystem.current.SetSelectedGameObject(null);
        GameManager.Instance.AudioManager.PlayOneShot(SFX.HOVER_BUTTON);
        ApplyWireModeText();
    }

    private void ApplyWireModeText()
    {
        var dm = GameManager.Instance.DataManager;
        if (dm.wiremode == WIREMODE.HOLD)
        {
            _text.SetText("Hold");
        }
        else if (dm.wiremode == WIREMODE.TOGGLE)
        {
            _text.SetText("Toggle");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
