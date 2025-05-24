using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ResultUI : UIWindow
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI gradeText;
    public CanvasGroup gradeCG;
    public CanvasGroup rankingCG;
    public Button restartButton;

    public int stage = 1;
    public string myName = "UNKNOWN";
    public float record = 1.111f;

    [SerializeField] private float animationDuration = 0f;
    [SerializeField] private float animationAfter = 0f;

    [SerializeField] private float timerDuration = 1f;
    [SerializeField] private float timerAfter = 1f;

    [SerializeField] private float gradeDuration = 1f;
    [SerializeField] private float gradeInitialScale = 2f;
    [SerializeField] private float gradeAfter = .5f;

    [SerializeField] private float showRankDuration = 1f;
    [SerializeField] private Vector3 rankingInitPos = new Vector3(100, 0, 0);
    [SerializeField] private float showRankAfter = 1f;

    [SerializeField] private float rankingDuration = 1f;
    [SerializeField] private float rankingAnimationUnit = 100f;
    [SerializeField] private float rankingAfter = 1f;

    [SerializeField] private float buttonDuration = 1f;

    private bool _timerAnimationFlag = false;
    private float _timerTimeElapsed = 0f;
    private LeaderBoardManager _leaderBoardManager;
    public List<RankingUIEntry> rankEntries;

    public void Restart()
    {
        Debug.Log("RESTART");
        //TODO - go to ingame scene
    }

    private string TimeToStr(float timeInSeconds)
    {
        if (timeInSeconds >= 6000) return "--:--.---";
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds - (minutes * 60 + seconds)) * 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
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

        StartCoroutine(Animate());
    }

    private void BeginTimerAnimation()
    {
        _timerAnimationFlag = true;
        timeText.gameObject.SetActive(true);
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
        Vector3 pos0 = rankingCG.transform.position;
        rankingCG.transform.position += rankingInitPos;
        rankingCG.alpha = 0;
        rankingCG.transform.DOMove(pos0, showRankDuration);
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
            rankEntries[0].transform.DOMoveY(rankEntries[0].transform.position.y + rankingAnimationUnit * (_leaderBoardManager.length - i + 2), rankingDuration);
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
                rankEntries[j].transform.DOMoveY(rankEntries[j].transform.position.y - rankingAnimationUnit, rankingDuration);
            }
        }
    }

    private void BeginButtonAnimation()
    {
        restartButton.transform.localScale = Vector3.zero;
        restartButton.gameObject.SetActive(true);
        restartButton.transform.DOScale(Vector3.one, buttonDuration).SetEase(Ease.OutBack);
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(animationDuration + animationAfter);

        BeginTimerAnimation();
        yield return new WaitForSeconds(timerDuration + timerAfter);

        BeginGradeAnimation();
        yield return new WaitForSeconds(gradeDuration + gradeAfter);

        BeginShowRankAnimation();
        yield return new WaitForSeconds(showRankDuration + showRankAfter);

        BeginRankAnimation();
        yield return new WaitForSeconds(rankingDuration + rankingAfter);

        BeginButtonAnimation();
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
