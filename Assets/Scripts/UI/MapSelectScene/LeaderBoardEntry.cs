using UnityEngine;

[System.Serializable]
public class LeaderBoardEntry
{
    public string name;
    public float timeInSeconds;

    public LeaderBoardEntry(string name, float timeInSeconds)
    {
        this.name = name;
        this.timeInSeconds = timeInSeconds;
    }

    public string GetName()
    {
        return name;
    }

    public string GetTime()
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = Mathf.RoundToInt((timeInSeconds - (minutes * 60 + seconds)) * 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}