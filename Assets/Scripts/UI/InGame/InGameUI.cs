using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    private float _elapsedTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _elapsedTime = 0;
    }

    private void UpdateTimeText()
    {
        int minutes = (int)(_elapsedTime / 60);
        int seconds = (int)(_elapsedTime % 60);
        int milliseconds = (int)((_elapsedTime - (minutes * 60 + seconds)) * 1000);
        timeText.SetText(string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds));
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        UpdateTimeText();
    }
}
