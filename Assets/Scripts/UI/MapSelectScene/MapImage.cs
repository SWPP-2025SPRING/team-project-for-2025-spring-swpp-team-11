using UnityEngine;
using DG.Tweening;

public class MapImage : MonoBehaviour
{
    public float W = 1920;
    public float duration = 0.5f;
    public float y0 = 150;
    public float y1 = 50;
    public CanvasGroup startUI;
    public float fadeDuration = 0.5f;

    public bool canStart { get; private set; } = false;

    private RectTransform _rect;
    private Tween _tween;
    private Tween _startTween;

    public void BeginMove(int p0, int p1)
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();
        if (p0 == 0)
        {
            _rect.anchoredPosition = new Vector2(p0 * W, y0);
            _tween = _rect.DOAnchorPos(new Vector2(p1 * W, y1), duration);
        }
        else
        {
            _rect.anchoredPosition = new Vector2(p0 * W, y1);
            _tween = _rect.DOAnchorPos(new Vector2(p1 * W, y0), duration);
        }
    }

    public void ShowStartUI()
    {
        if (canStart || (_tween != null && _tween.IsActive())) return;
        canStart = true;
        if (_startTween != null && _startTween.IsActive())
            _startTween.Kill();
        startUI.gameObject.SetActive(true);
        _startTween = startUI.DOFade(1, fadeDuration);
    }

    public void HideStartUI()
    {
        if (!canStart) return;
        canStart = false;
        if (_startTween != null && _startTween.IsActive())
            _startTween.Kill();
        _startTween = startUI.DOFade(0, fadeDuration);
        _startTween.OnComplete(() => { startUI.gameObject.SetActive(false); });
    }

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
    }
}