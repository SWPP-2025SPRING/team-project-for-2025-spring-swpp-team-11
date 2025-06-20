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
    public GradeCutManager GradeCutManager { get; private set; }
    public WIREMODE wiremode { get; private set; } = WIREMODE.HOLD;
    public float sensitivity { get; private set; } = 1.0f;

    public void SetWireMode(WIREMODE wiremode)
    {
        this.wiremode = wiremode;
    }

    public void SetSensitivity(float sensitivity)
    {
        this.sensitivity = sensitivity;
    }

    public int selectedStage = 1;

    private void Start()
    {
        LeaderBoardManager = GetComponentInChildren<LeaderBoardManager>();
        GradeCutManager = GetComponentInChildren<GradeCutManager>();
    }
}
