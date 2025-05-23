using UnityEngine;


public class LeaderBoardManager : MonoBehaviour
{
    public const string Key = "leaderboard";

    public int stageNo = 3;
    public int length = 3;

    private LeaderBoardContent[] _leaderboard;

    void Start()
    {
        _leaderboard = new LeaderBoardContent[stageNo];
        for(int i = 1; i <= stageNo; i++)
        {
            string record = PlayerPrefs.GetString(Key + i, "{\"entries\":[]}");
            _leaderboard[i - 1] = LeaderBoardContent.FromJson(record);
        }
    }
    
    public string GetSingleName(int stage, int i)
    {
        return _leaderboard[stage - 1].GetSingleName(i - 1);
    }

    public float GetSingleTime(int stage,int i)
    {
        return _leaderboard[stage - 1].GetSingleTime(i - 1);
    }

    public string GetNameStr(int stage)
    {
        return _leaderboard[stage - 1].GetNameStr(length);
    }

    public string GetTimeStr(int stage)
    {
        return _leaderboard[stage - 1].GetTimeStr(length);
    }

    public void AddRecord(int stage, string name, float time)
    {
        int i = stage - 1;
        _leaderboard[i].AddRecord(name, time, length);
        PlayerPrefs.SetString(Key + stage, JsonUtility.ToJson(_leaderboard[i]));
    }

    public void ClearRecords()
    {
        for (int stage = 1; stage <= stageNo; stage++) 
        {
            PlayerPrefs.DeleteKey(Key + stage);
            _leaderboard[stage - 1].ClearRecords();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
