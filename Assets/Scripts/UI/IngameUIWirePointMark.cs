using UnityEngine;
using UnityEngine.UI;

public class IngameUIWirePointMark : MonoBehaviour
{
    public Image wireMarkImage;
    public RectTransform ingameUiRectTransform;

    public void SetWireMarkImage(Vector3 worldWirePos)
    {
        Vector2 localPos;
        var uiWireMarkimagepos = Camera.main.WorldToScreenPoint(worldWirePos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            ingameUiRectTransform,
            uiWireMarkimagepos,
            null,
            out localPos);
        wireMarkImage.rectTransform.anchoredPosition = localPos;
    }

    public void MakeMarkOn()
    {
        wireMarkImage.gameObject.SetActive(true);
    }

    public void MakeMarkOff()
    {
        wireMarkImage.gameObject.SetActive(false);
    }
}
