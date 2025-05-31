using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleManager : UIWindow
{
    public List<ThemeButton> menuButtons;
    private int _currentIndex = 0;

    /* ---------- Unity lifecycle ---------- */
    protected override void Start()
    {
        base.Start();

        SelectButton(_currentIndex);

        onEnterDown.AddListener(OnEnter);
        onVerticalDown.AddListener(OnVerticalDown);
    }

    private void OnVerticalDown(int v)
    {
        _currentIndex = (_currentIndex - v + menuButtons.Count) % menuButtons.Count;
        SelectButton(_currentIndex);
    }

    private void OnEnter()
    {
        menuButtons[_currentIndex].button.onClick.Invoke();








    }

    /* ---------- 공용 선택 로직 ---------- */
    private void SelectButton(int index)
    {
        // Unity 내장 Selectable 시스템 활용 → 마우스·키보드 색상 통일
        menuButtons[index].Select();
        UpdateColors(index);
    }

    private void UpdateColors(int selected)
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            if (i == selected) menuButtons[i].Select();
            else menuButtons[i].Unselect();







        }
    }

    /* ---------- EventTrigger에서 호출 ---------- */
    // **Point Enter** 에 연결
    public void OnPointerEnter(BaseEventData data)
    {
        var ped = data as PointerEventData;
        if (ped == null) return;

        // Text, Image 같은 자식 오브젝트에서도 Button을 찾도록
        var btn = ped.pointerEnter?.GetComponentInParent<ThemeButton>();
        if (btn == null) return;

        _currentIndex = menuButtons.IndexOf(btn);
        SelectButton(_currentIndex);   // 색상 재계산
    }


    // **Point Exit** (선택적) – 나가더라도 마지막으로 선택된 버튼 유지
    public void OnPointerExit(BaseEventData data)
    {
        UpdateColors(_currentIndex);
    }

    /* ---------- 버튼 클릭 콜백 ---------- */
    public void StartGame() => StartCoroutine(GameManager.Instance.SceneLoadManager.FadeAndLoadScene("StageSelectSceneAppliedOne"));
    public void Option() => Debug.Log("Option 버튼 눌림");
    public void ExitGame() => Application.Quit();
}
