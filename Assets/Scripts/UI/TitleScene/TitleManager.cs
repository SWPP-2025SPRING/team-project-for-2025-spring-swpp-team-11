using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : UIWindow
{
    public List<Button> menuButtons;
    private int _currentIndex = 0;

    protected override void Start()
    {
        base.Start();
        
        onEnterDown.AddListener(OnEnterDown);
        onVerticalDown.AddListener(OnVerticalDown);
        
        HighlightButton(_currentIndex);
    }
    
    private void OnEnterDown()
    {
        menuButtons[_currentIndex].onClick.Invoke();
    }
    
    private void OnVerticalDown(int direction)
    {
        _currentIndex = (_currentIndex - direction + menuButtons.Count) % menuButtons.Count;
        HighlightButton(_currentIndex);
    }

    private void HighlightButton(int index)
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            ColorBlock colors = menuButtons[i].colors;
            colors.normalColor = (i == index) ? Color.green : Color.white;
            menuButtons[i].colors = colors;
        }
    }

    // 버튼 클릭 시 실행
    public void StartGame()
    {
        StartCoroutine(GameManager.Instance.SceneLoadManager.FadeAndLoadScene("GameScene")); // GameScene으로 이동
    }

    public void Option()
    {
        Debug.Log("Option 버튼 눌림");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
