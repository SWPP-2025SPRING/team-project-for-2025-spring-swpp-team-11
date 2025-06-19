using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : UIWindow
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI gradeText;
    public CanvasGroup gradeCG;
    public RectTransform rankingRect;
    public CanvasGroup rankingCG;
    public List<RankingUIEntry> rankEntries;
    public Button restartButton;
    public Button exitButton;

    public int stage = 1;
    public string myName = "UNKNOWN";
    public float record = 1.111f;

    [SerializeField] private float animationDuration = 0f;
    [SerializeField] private float animationAfter = 0f;

    [SerializeField] private float timerDuration = 1f;
    [SerializeField] private float timerAfter = 1f;

    [SerializeField] private float gradeDuration = 1f;
    [SerializeField] private float gradeAudioOffset = .2f;
    [SerializeField] private float gradeInitialScale = 2f;
    [SerializeField] private float gradeAfter = .5f;

    [SerializeField] private float showRankDuration = 1f;
    [SerializeField] private Vector2 rankingInitPos = new Vector3(100, 0);
    [SerializeField] private float showRankAfter = 1f;

    [SerializeField] private float rankingDuration = 1f;
    [SerializeField] private float rankingAnimationUnit = 100f;
    [SerializeField] private float rankingAfter = 1f;

    [SerializeField] private float buttonDuration = 1f;

    private bool _timerAnimationFlag = false;
    private float _timerTimeElapsed = 0f;
    private LeaderBoardManager _leaderBoardManager;

    public string mapSelectSceneName;

    public void Restart()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SceneLoadManager.FadeLoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void Exit()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SceneLoadManager.FadeLoadScene(mapSelectSceneName);
    }

    private string TimeToStr(float timeInSeconds)
    {
        if (timeInSeconds >= 6000) return "--:--.---";
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds - (minutes * 60 + seconds)) * 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void SetAnimationDuration(float duration)
    {
        animationDuration = duration;
    }

    protected override void Start()
    {
        base.Start();

        _leaderBoardManager = GameManager.Instance.DataManager.LeaderBoardManager;
        for(int i = 1; i <= _leaderBoardManager.length; i++)
        {
            string name = i + "  " + _leaderBoardManager.GetSingleName(stage, i);
            float time = _leaderBoardManager.GetSingleTime(stage, i);
            rankEntries[i].Initialize(name, time);
        }
        rankEntries[0].Initialize("-  " + myName, record);

        //decomment this to submit the record
        //_leaderBoardManager.AddRecord(stage, myName, record);
    }

    private void BeginTimerAnimation()
    {
        _timerAnimationFlag = true;
        timeText.gameObject.SetActive(true);
        GameManager.Instance.AudioManager.PlayOneShot(SFX.TIME_INCREASE);
    }

    private void BeginGradeAnimation()
    {
        gradeText.gameObject.SetActive(true);
        gradeText.transform.localScale = new Vector3(gradeInitialScale, gradeInitialScale, gradeInitialScale);
        gradeCG.alpha = 0;
        gradeText.transform.DOScale(new Vector3(1, 1, 1), gradeDuration);
        gradeCG.DOFade(1, gradeDuration);
    }

    private void BeginShowRankAnimation()
    {
        rankingCG.gameObject.SetActive(true);
        Vector2 pos = rankingRect.anchoredPosition;
        rankingRect.anchoredPosition += rankingInitPos;
        rankingCG.alpha = 0;
        rankingRect.DOAnchorPos(pos, showRankDuration);
        rankingCG.DOFade(1, showRankDuration);
    }

    private void BeginRankAnimation()
    {
        int i = _leaderBoardManager.length + 1;
        while (i > 1)
        {
            if (_leaderBoardManager.GetSingleTime(stage, i - 1) <= record) break;
            i--;
        }
        if (i <= _leaderBoardManager.length)
        {
            rankEntries[0].SetName(i + "  " + myName);
            RectTransform rect = rankEntries[0].GetComponent<RectTransform>();
            rect.DOAnchorPosY(rect.anchoredPosition.y + rankingAnimationUnit * (_leaderBoardManager.length - i + 2), rankingDuration);
        }
        for(int j = i; j <= _leaderBoardManager.length; j++)
        {
            if (j == _leaderBoardManager.length)
            {
                rankEntries[j].GetComponent<CanvasGroup>().DOFade(0, rankingDuration);
            }
            else
            {
                rankEntries[j].SetRank(j + 1);
                RectTransform rect = rankEntries[j].GetComponent<RectTransform>();
                rect.DOAnchorPosY(rect.anchoredPosition.y - rankingAnimationUnit, rankingDuration);
            }
        }
    }

    private void BeginButtonAnimation()
    {
        restartButton.transform.localScale = Vector3.zero;
        restartButton.gameObject.SetActive(true);
        restartButton.transform.DOScale(Vector3.one, buttonDuration).SetEase(Ease.OutBack);
    }
    
    private void BeginExitButtonAnimation()
    {
        exitButton.transform.localScale = Vector3.zero;
        exitButton.gameObject.SetActive(true);
        exitButton.transform.DOScale(Vector3.one, buttonDuration).SetEase(Ease.OutBack);
    }

    public IEnumerator Animate()
    {
        GameManager.Instance.AudioManager.PlayOneShot(SFX.RESULT);
        yield return new WaitForSeconds(animationDuration + animationAfter);

        BeginTimerAnimation();
        yield return new WaitForSeconds(timerDuration + timerAfter);

        BeginGradeAnimation();
        yield return new WaitForSeconds(gradeDuration - gradeAudioOffset);

        GameManager.Instance.AudioManager.PlayOneShot(SFX.GRADE_EXPLODE);
        yield return new WaitForSeconds(gradeAfter + gradeAudioOffset);

        BeginShowRankAnimation();
        yield return new WaitForSeconds(showRankDuration + showRankAfter);

        BeginRankAnimation();
        GameManager.Instance.DataManager.LeaderBoardManager.AddRecord(stage, "TMP(TOBEFIXED)", record);
        yield return new WaitForSeconds(rankingDuration + rankingAfter);
        

        BeginButtonAnimation();
        yield return new WaitForSeconds(0.2f);
        BeginExitButtonAnimation();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_timerAnimationFlag)
        {
            _timerTimeElapsed = Mathf.Min(_timerTimeElapsed + Time.deltaTime, timerDuration);
            float timeToDisplay = record * _timerTimeElapsed / timerDuration;
            timeText.SetText(TimeToStr(timeToDisplay));
        }
    }
}
