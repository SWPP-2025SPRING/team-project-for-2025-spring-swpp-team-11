using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ResultUI : UIWindow
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI gradeText;
    public CanvasGroup gradeCG;

    public float record = 671.111f;

    [SerializeField] private float animationDuration = 0f;
    [SerializeField] private float animationAfter = 0f;

    [SerializeField] private float timerDuration = 1f;
    [SerializeField] private float timerAfter = 1f;

    [SerializeField] private float gradeDuration = 1f;
    [SerializeField] private float gradeInitialScale = 2f;

    private bool _timerAnimationFlag = false;
    private float _timerTimeElapsed = 0f;

    private string TimeToSec(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds - (minutes * 60 + seconds)) * 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(animationDuration + animationAfter);

        _timerAnimationFlag = true;
        timeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(timerDuration + timerAfter);

        gradeText.gameObject.SetActive(true);
        gradeText.transform.localScale = new Vector3(gradeInitialScale, gradeInitialScale, gradeInitialScale);
        gradeCG.alpha = 0;
        gradeText.transform.DOScale(new Vector3(1, 1, 1), gradeDuration);
        gradeCG.DOFade(1, gradeDuration);
        yield return new WaitForSeconds(gradeDuration);
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerAnimationFlag)
        {
            _timerTimeElapsed = Mathf.Min(_timerTimeElapsed + Time.deltaTime, timerDuration);
            float timeToDisplay = record * _timerTimeElapsed / timerDuration;
            timeText.SetText(TimeToSec(timeToDisplay));
        }
    }
}
