using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderBoardContent
{
    public List<LeaderBoardEntry> entries;
    public LeaderBoardContent(List<LeaderBoardEntry> entries)
    {
        this.entries = entries;
    }

    public string GetNameStr(int maxLen)
    {
        string str = string.Empty;
        int i = 0;
        foreach (LeaderBoardEntry entry in entries)
        {
            i++;
            str += i + "  " + entry.GetName() + "\n";
            if (i == maxLen) break;
        }
        while (i < maxLen)
        {
            str += "--------\n";
            i++;
        }
        return str;
    }

    public string GetTimeStr(int maxLen)
    {
        string str = string.Empty;
        int i = 0;
        foreach (LeaderBoardEntry entry in entries)
        {
            str += entry.GetTime() + "\n";
            i++;
            if (i == maxLen) break;
        }
        while (i < maxLen)
        {
            str += "--:--.---\n";
            i++;
        }
        return str;
    }

    public string GetSingleName(int i)
    {
        if (i < entries.Count)
            return entries[i].name;
        else
            return "--------";
    }

    public float GetSingleTime(int i)
    {
        if (i < entries.Count)
            return entries[i].timeInSeconds;
        else
            return float.PositiveInfinity;
    }

    public void AddRecord(string name, float time, int maxLen)
    {
        int i = entries.Count;
        while (i > 0)
        {
            if (entries[i - 1].timeInSeconds <= time) break;
            i--;
        }
        LeaderBoardEntry newEntry = new LeaderBoardEntry(name, time);
        entries.Insert(i, newEntry);
        while (entries.Count > maxLen)
        {
            entries.RemoveAt(entries.Count - 1);
        }
    }

    public void ClearRecords()
    {
        entries.Clear();
    }

    public static LeaderBoardContent FromJson(string jsonString)
    {
        return JsonUtility.FromJson<LeaderBoardContent>(jsonString);
    }
}