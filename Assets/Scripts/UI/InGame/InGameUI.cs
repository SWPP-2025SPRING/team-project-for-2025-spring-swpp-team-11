using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUI : UIWindow
{
    public TextMeshProUGUI timeText;
    public GameObject pauseUI;

    private float _elapsedTime;
    private bool _paused;

    protected override void Start()
    {
        base.Start();

        _elapsedTime = 0;
        _paused = false;

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


    protected void Update()
    {
        _elapsedTime += Time.deltaTime;
        UpdateTimeText();
    }

    private void OnEscape()
    {
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
}
