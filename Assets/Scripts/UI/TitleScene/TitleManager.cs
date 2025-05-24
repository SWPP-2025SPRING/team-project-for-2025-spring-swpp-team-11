using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public List<Button> menuButtons;
    private int currentIndex = 0;

    /* ---------- Unity lifecycle ---------- */
    private void Start()
    {
        SelectButton(currentIndex);   // 처음 버튼 선택
    }

    private void Update()
    {
        // ↓↓ 키보드 네비게이션 ↓↓
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % menuButtons.Count;
            SelectButton(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + menuButtons.Count) % menuButtons.Count;
            SelectButton(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            menuButtons[currentIndex].onClick.Invoke();
        }
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
            Color tint = (i == selected) ? Color.green : Color.white;

            ColorBlock cb = menuButtons[i].colors;
            cb.normalColor = tint;   // 기본
            cb.highlightedColor = tint;   // 마우스 호버
            cb.selectedColor = tint;   // 키보드/게임패드 선택
            cb.pressedColor = new Color(tint.r * 0.7f, tint.g * 0.7f, tint.b * 0.7f);

            menuButtons[i].colors = cb;
        }
    }

    /* ---------- EventTrigger에서 호출 ---------- */
    // **Point Enter** 에 연결
    public void OnPointerEnter(BaseEventData data)
    {
        var ped = data as PointerEventData;
        if (ped == null) return;

        // Text, Image 같은 자식 오브젝트에서도 Button을 찾도록
        var btn = ped.pointerEnter?.GetComponentInParent<Button>();
        if (btn == null) return;

        currentIndex = menuButtons.IndexOf(btn);
        SelectButton(currentIndex);   // 색상 재계산
    }


    // **Point Exit** (선택적) – 나가더라도 마지막으로 선택된 버튼 유지
    public void OnPointerExit(BaseEventData data)
    {
        UpdateColors(currentIndex);
    }

    /* ---------- 버튼 클릭 콜백 ---------- */
    public void StartGame() => StartCoroutine(FadeManager.Instance.FadeAndLoadScene("GameScene"));
    public void Option() => Debug.Log("Option 버튼 눌림");
    public void ExitGame() => Application.Quit();
}