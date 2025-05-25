using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectUI : UIWindow
{
    public List<MapImage> mapImages;

    public TextMeshProUGUI mapTitle;
    public TextMeshProUGUI leaderBoardName;
    public TextMeshProUGUI leaderBoardTime;

    public TMP_InputField nameInput;
    public TMP_InputField timeInput;

    private int _currentSelectedStage = 1;
    private LeaderBoardManager _leaderBoardManager;
    
    protected override void Start()
    {
        base.Start();
        _leaderBoardManager = GameManager.Instance.DataManager.LeaderBoardManager;
        
        onEnterDown.AddListener(OnEnterDown);
        onHorizontalDown.AddListener(OnHorizontalDown);
        
        ApplyUIUpdate();
    }

    private void ApplyUIUpdate()
    {
        leaderBoardName.SetText(_leaderBoardManager.GetNameStr(_currentSelectedStage));
        leaderBoardTime.SetText(_leaderBoardManager.GetTimeStr(_currentSelectedStage));
    }

    public void UpdateSelectedStage(int direction)
    {
        int newStage = (_currentSelectedStage + direction);
        if (newStage < 1) newStage += mapImages.Count;
        if (newStage > mapImages.Count) newStage -= mapImages.Count;
        mapImages[_currentSelectedStage - 1].BeginMove(0, -direction);
        mapImages[_currentSelectedStage - 1].HideStartUI();
        mapImages[newStage - 1].BeginMove(direction, 0);
        _currentSelectedStage = newStage;
        ApplyUIUpdate();
    }

    public void StartGame()
    {
        Debug.Log("Stage " + _currentSelectedStage + " start");
        //TODO - start game
        //SceneManager.LoadScene(...)
    }

    private void OnEnterDown()
    {
        if (mapImages[_currentSelectedStage - 1].canStart)
        {
            StartGame();
        }
        else
        {
            mapImages[_currentSelectedStage - 1].ShowStartUI();
        }
    }

    private void OnHorizontalDown(int v)
    {
        Debug.Log("ASDGSAG");
        UpdateSelectedStage(v);
    }

    public void OnLeaderBoardSubmit()
    {
        string name = nameInput.text;
        float time = float.Parse(timeInput.text);
        _leaderBoardManager.AddRecord(_currentSelectedStage, name, time);
        ApplyUIUpdate();
    }

    public void OnLeaderBoardClear()
    {
        _leaderBoardManager.ClearRecords();
        ApplyUIUpdate();
    }
}
