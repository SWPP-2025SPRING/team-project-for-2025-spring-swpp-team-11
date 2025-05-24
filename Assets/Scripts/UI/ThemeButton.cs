using UnityEngine;
using UnityEngine.UI;

public class ThemeButton : MonoBehaviour
{
    public Sprite basic;
    public Sprite selected;

    [System.NonSerialized] public Button button;

    private Image _image;

    void Awake()
    {
        button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    public void Unselect()
    {
        _image.sprite = basic;
    }

    public void Select()
    {
        _image.sprite = selected;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
