using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreKeeper: MonoBehaviour
{
    [SerializeField] int randomUpdateRange;
    [SerializeField] float randomUpdatePeriod;

    public static int Revenue { get; private set; }
    public static int StrikesLeft { get; private set; } = 3;

    public static EventHandler FluctuateIncomeEventHandler;

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

    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.myGameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                Revenue = 100;
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
            case DayManager.DayState.StartDay:
                StrikesLeft = 3;
            break;
        }
    }

    void HandleHiredCandidate(object sender, ReviewedCandidateEventArgs e)
    {
        int hiredMultiplier = e.wasDecisionCorrect ? 1 : -1;
        Revenue += 10 * hiredMultiplier;

        if (!e.wasDecisionCorrect)
            StrikesLeft--;
    }

    IEnumerator FluctuateRevenue()
    {
        FluctuateIncomeEventHandler?.Invoke(this, new());

        Revenue += Random.Range(-randomUpdateRange / 2, randomUpdateRange / 2);
        yield return new WaitForSeconds(randomUpdatePeriod);
        StartCoroutine(FluctuateRevenue());
    }

}
