using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//it will be changed to UIWindow
public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public GameObject pauseUI;

    private PlayerInputProcessor _inputProcessor;

    private float _elapsedTime;
    private bool _paused;

    private StageManager _stageManager;

    protected void Start()
    {
        _elapsedTime = 0;
        _paused = false;
        _inputProcessor = FindFirstObjectByType<PlayerInputProcessor>();
        _inputProcessor.escapeEvent.AddListener(OnEscape);
        
        _stageManager = FindFirstObjectByType<StageManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateTimeText()
    {
        int minutes = (int)(_elapsedTime / 60);
        int seconds = (int)(_elapsedTime % 60);
        int milliseconds = (int)((_elapsedTime - (minutes * 60 + seconds)) * 1000);
        timeText.SetText(string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds));
    }

    public void SetPause(bool paused)
    {
        _paused = paused;
    }

    private void OnDisable()
    {
        _inputProcessor.escapeEvent.RemoveListener(OnEscape);
    }


    protected void Update()
    {
        if (_stageManager.currentStageState == StageState.Started)
            _elapsedTime += Time.deltaTime;
        UpdateTimeText();
    }

    private void OnEscape()
    {
        if (_paused)
        {
            ResumeGame();
            return;
        }
        _paused = true;
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResumeGame()
    {
        _paused = false;
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool GetPaused() { return _paused; }
    public float GetElapsedTime() { return _elapsedTime; }
}
