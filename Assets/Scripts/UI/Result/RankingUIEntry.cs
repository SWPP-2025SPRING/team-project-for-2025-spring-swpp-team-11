using TMPro;
using UnityEngine;

public class RankingUIEntry : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeText;

    private string _name;
    private float _time;

    private string TimeToStr(float timeInSeconds)
    {
        if (timeInSeconds >= 6000) return "--:--.---";
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds - (minutes * 60 + seconds)) * 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    private void ApplyText()
    {
        nameText.SetText(_name);
        timeText.SetText(TimeToStr(_time));
    }

    public void Initialize(string name, float time)
    {
        _name = name;
        _time = time;
        ApplyText();
    }

    public void SetName(string name)
    {
        _name = name;
        ApplyText();
    }

    public void SetRank(int r)
    {
        _name = r + _name.Substring(1);
        ApplyText();
    }
}
