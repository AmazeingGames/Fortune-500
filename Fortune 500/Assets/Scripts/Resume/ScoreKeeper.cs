using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreKeeper: Singleton<ScoreKeeper>
{
    [SerializeField] int randomUpdateRange;
    [SerializeField] float randomUpdatePeriod;

    [SerializeField] EnvironmentData environmentData;

    public int Revenue { get; private set; }
    public int StrikesLeft { get; private set; } = 3;
    public int DayCount { get; private set; } = 0;

    private void OnEnable()
    {
        GameFlowManager.PerformActionEventHandler += HandleGameAction;
        CandidateHandler.HireCandidateEventHandler += HandleHiredCandidate;
    }

    private void OnDisable()
    {
        GameFlowManager.PerformActionEventHandler -= HandleGameAction;
        CandidateHandler.HireCandidateEventHandler -= HandleHiredCandidate;
    }

    void Start()
    {
        Revenue = 100;
        environmentData.StrikesLeftText.text = "III";
        StartCoroutine(FluctuateRevenue());
    }

    private void Update()
        => environmentData.RevenueText.text = "Revenue:" + Revenue + " bn";


    void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.StartWork:
                DayCount++;
                environmentData.DayText.text = $"{DayCount}";
            break;
        }
    }

    void HandleHiredCandidate(object sender, HiredCandidateEventArgs e)
    {
        int hiredMultiplier = e.wasDecisionCorrect ? 1 : 0;
        Revenue += 10 * hiredMultiplier;
        environmentData.RevenueText.text = "Revenue:" + Revenue + " bn";

        if (!e.wasDecisionCorrect)
        {
            StrikesLeft--;
            environmentData.StrikesLeftText.text = "";

            for (int i = 0; i < StrikesLeft; i++)
                environmentData.StrikesLeftText.text += "I";
        }
    }

    IEnumerator FluctuateRevenue()
    {
        Revenue += Random.Range(-randomUpdateRange / 2, randomUpdateRange / 2);
        environmentData.RevenueText.text = "Revenue:" + Revenue + " bn";
        yield return new WaitForSeconds(randomUpdatePeriod);
        StartCoroutine(FluctuateRevenue());
    }

}
