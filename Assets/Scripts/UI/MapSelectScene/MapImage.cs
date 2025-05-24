using UnityEngine;
using DG.Tweening;

public class MapImage : MonoBehaviour
{
    public float W = 1920;
    public float duration = 0.5f;

    private RectTransform _rect;
    private Tween _tween;

    public void BeginMove(int p0, int p1)
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();
        _rect.anchoredPosition = new Vector2(p0 * W, _rect.anchoredPosition.y);
        _tween = _rect.DOAnchorPosX(p1 * W, duration);
    }

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
    }
}
