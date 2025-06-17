using System;
using UnityEngine;

public enum WIREMODE
{
    HOLD,
    TOGGLE,
}

public class DataManager : MonoBehaviour
{
    public LeaderBoardManager LeaderBoardManager { get; private set; }
    public WIREMODE wiremode { get; private set; } = WIREMODE.HOLD;

    public void SetWireMode(WIREMODE wiremode)
    {
        this.wiremode = wiremode;
    }

    private void Start()
    {
        LeaderBoardManager = GetComponentInChildren<LeaderBoardManager>();
    }
}
