using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectUI : UIWindow
{
    public List<Sprite> mapImages;
    public GameObject mapImageObj;
    public GameObject gameStartUI;

    public TextMeshProUGUI mapTitle;
    public TextMeshProUGUI leaderBoardName;
    public TextMeshProUGUI leaderBoardTime;

    public TMP_InputField nameInput;
    public TMP_InputField timeInput;

    private int _currentSelectedStage = 1;
    private bool _isWaiting = false;
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
        mapImageObj.GetComponent<Image>().sprite = mapImages[_currentSelectedStage - 1];
        leaderBoardName.SetText(_leaderBoardManager.GetNameStr(_currentSelectedStage));
        leaderBoardTime.SetText(_leaderBoardManager.GetTimeStr(_currentSelectedStage));
    }

    public void UpdateSelectedStage(int direction)
    {
        _currentSelectedStage = (_currentSelectedStage + direction);
        if (_currentSelectedStage < 1) _currentSelectedStage += mapImages.Count;
        if (_currentSelectedStage > mapImages.Count) _currentSelectedStage -= mapImages.Count;
        _isWaiting = false;
        gameStartUI.SetActive(false);
        ApplyUIUpdate();
    }

    public void OnMapImageClick()
    {
        if (!_isWaiting)
        {
            _isWaiting = true;
            mapTitle.SetText("Stage " + _currentSelectedStage);
            gameStartUI.SetActive(true);
        }
    }

    public void StartGame()
    {
        Debug.Log("Stage " + _currentSelectedStage + " start");
        //TODO - start game
        //SceneManager.LoadScene(...)
    }

    private void OnEnterDown()
    {
        if (!_isWaiting)
        {
            OnMapImageClick();
        }
        else
        {
            StartGame();
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
