using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public SFX sfx = SFX.CLICK_BUTTON;

    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            GameManager.Instance.AudioManager.PlayOneShot(sfx);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
