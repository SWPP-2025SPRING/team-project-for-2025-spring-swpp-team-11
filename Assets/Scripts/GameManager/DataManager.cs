using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public LeaderBoardManager LeaderBoardManager { get; private set; }

    public int selectedStage = 2;

    private void Start()
    {
        LeaderBoardManager = GetComponentInChildren<LeaderBoardManager>();
    }
}
