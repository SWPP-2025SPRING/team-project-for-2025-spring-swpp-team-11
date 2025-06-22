using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleManager : UIWindow
{
    public List<ThemeButton> menuButtons;
    public GameObject setupUI;
    private int _currentIndex = 0;

    /* ---------- Unity lifecycle ---------- */

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    protected override void Start()
    {
        base.Start();

        SelectButton(_currentIndex);

        onEnterDown.AddListener(OnEnter);
        onVerticalDown.AddListener(OnVerticalDown);
    }

    private void OnVerticalDown(int v)
    {
        int newIndex = (_currentIndex - v + menuButtons.Count) % menuButtons.Count;
        SelectButton(newIndex);
    }

    private void OnEnter()
    {
        menuButtons[_currentIndex].button.onClick.Invoke();
    }

    /* ---------- 공용 선택 로직 ---------- */
    private void SelectButton(int index)
    {
        // Unity 내장 Selectable 시스템 활용 → 마우스·키보드 색상 통일
        if(_currentIndex != index)
            GameManager.Instance.AudioManager.PlayOneShot(SFX.HOVER_BUTTON);
        _currentIndex = index;
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

        int newIndex = menuButtons.IndexOf(btn);
        SelectButton(newIndex);   // 색상 재계산
    }


    // **Point Exit** (선택적) – 나가더라도 마지막으로 선택된 버튼 유지
    public void OnPointerExit(BaseEventData data)
    {
        UpdateColors(_currentIndex);
    }

    /* ---------- 버튼 클릭 콜백 ---------- */
    public void StartGame() => GameManager.Instance.SceneLoadManager.FadeLoadScene("1_StageSelectSceneAppliedOne");
    public void Option() {
        if (setupUI.activeSelf) setupUI.SetActive(false);
        else setupUI.SetActive(true);
    }
    public void ExitGame() => Application.Quit();
}
