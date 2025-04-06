using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreKeeper: Singleton<ScoreKeeper>
{
    [SerializeField] int randomUpdateRange;
    [SerializeField] float randomUpdatePeriod;

    public int Revenue { get; private set; }
    public int StrikesLeft { get; private set; } = 3;
    public int DayCount { get; private set; } = 0;

    private void OnEnable()
    {
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
        CandidateHandler.ReviewedCandidateEventHandler += HandleHiredCandidate;
        GameFlowManager.PerformGameActionEventHandler += HandlePerformGameAction;
    }

    private void OnDisable()
    {
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
        CandidateHandler.ReviewedCandidateEventHandler -= HandleHiredCandidate;
        GameFlowManager.PerformGameActionEventHandler -= HandlePerformGameAction;
    }

    private void Update()
        => EnvironmentData.Instance.RevenueText.text = "Revenue:" + Revenue + " bn";


    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                Revenue = 100;
                EnvironmentData.Instance.StrikesLeftText.text = "III";
                StartCoroutine(FluctuateRevenue());
            break;

            case GameFlowManager.GameAction.PauseGame:
            case GameFlowManager.GameAction.ResumeGame:
            case GameFlowManager.GameAction.LoseGame:
            break;
        }
    }

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayManager.DayState.StartWork:
                DayCount++;
                EnvironmentData.Instance.DayText.text = $"{DayCount}";
            break;

            case DayManager.DayState.StartDay:
                StrikesLeft = 3;
            break;
        }
    }

    void HandleHiredCandidate(object sender, ReviewedCandidateEventArgs e)
    {
        int hiredMultiplier = e.wasDecisionCorrect ? 1 : 0;
        Revenue += 10 * hiredMultiplier;
        EnvironmentData.Instance.RevenueText.text = "Revenue:" + Revenue + " bn";

        if (!e.wasDecisionCorrect)
        {
            StrikesLeft--;
            EnvironmentData.Instance.StrikesLeftText.text = "";

            for (int i = 0; i < StrikesLeft; i++)
                EnvironmentData.Instance.StrikesLeftText.text += "I";
        }
    }

    IEnumerator FluctuateRevenue()
    {
        Revenue += Random.Range(-randomUpdateRange / 2, randomUpdateRange / 2);
        EnvironmentData.Instance.RevenueText.text = "Revenue:" + Revenue + " bn";
        yield return new WaitForSeconds(randomUpdatePeriod);
        StartCoroutine(FluctuateRevenue());
    }

}
