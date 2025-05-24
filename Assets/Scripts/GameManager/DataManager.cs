using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public LeaderBoardManager LeaderBoardManager { get; private set; }

    private void Start()
    {
        LeaderBoardManager = GetComponentInChildren<LeaderBoardManager>();
    }
}
