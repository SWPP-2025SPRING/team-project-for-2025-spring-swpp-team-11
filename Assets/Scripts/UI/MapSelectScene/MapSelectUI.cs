using System.Collections;
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

    [SerializeField] private string[] playSceneNames;
    
    protected override void Start()
    {
        base.Start();
        _leaderBoardManager = FindAnyObjectByType<LeaderBoardManager>();
        
        onEnterDown.AddListener(OnEnterDown);
        onHorizontalDown.AddListener(OnHorizontalDown);
        
        ApplyUIUpdate();

        StartCoroutine(UpdateIndex());
    }
    

    private void ApplyUIUpdate()
    {
        Debug.Log(_currentSelectedStage);
        leaderBoardName.SetText(_leaderBoardManager.GetNameStr(_currentSelectedStage));
        leaderBoardTime.SetText(_leaderBoardManager.GetTimeStr(_currentSelectedStage));
    }

    private IEnumerator UpdateIndex()
    {
        yield return new WaitForSeconds(0.1f);
        int newIndex = GameManager.Instance.DataManager.selectedStage;
        if (newIndex < 1) newIndex += mapImages.Count;
        if (newIndex > mapImages.Count) newIndex -= mapImages.Count;
        while (_currentSelectedStage != newIndex)
        {
            Debug.Log("ASDG");
            UpdateSelectedStage(1);
            yield return new WaitForSeconds(0.05f);
        }
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
        GameManager.Instance.SceneLoadManager.FadeLoadScene(playSceneNames[_currentSelectedStage - 1]);
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
