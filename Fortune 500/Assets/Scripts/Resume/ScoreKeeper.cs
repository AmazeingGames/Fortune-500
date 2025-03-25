using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreKeeper: Singleton<ScoreKeeper>
{
    [SerializeField] int randomUpdateRange;
    [SerializeField] float randomUpdatePeriod;

    [SerializeField] TextMeshPro revenueText;
    [SerializeField] TextMeshPro strikesLeftText;
    [SerializeField] TextMeshPro dayText;

    public int Revenue { get; private set; }
    public int StrikesLeft { get; private set; } = 3;
    public int DayCount { get; private set; } = 0;

    private void OnEnable()
    {
        GameFlowManager.GameActionEventHandler += HandleGameAction;
        CandidateHandler.HiredCandidateEventHandler += OnHireCandidate;
    }

    private void OnDisable()
    {
        GameFlowManager.GameActionEventHandler -= HandleGameAction;
        CandidateHandler.HiredCandidateEventHandler -= OnHireCandidate;
    }

    void Start()
    {
        Revenue = 100;
        strikesLeftText.text = "III";
        StartCoroutine(FluctuateRevenue());
    }

    private void Update()
        => revenueText.text = "Revenue:" + Revenue + " bn";


    void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.StartWork:
                DayCount++;
                dayText.text = $"{DayCount}";
            break;
        }
    }

    void OnHireCandidate(object sender, HiredCandidateEventArgs e)
    {
        int hiredMultiplier = e.wasDecisionCorrect ? 1 : 0;
        Revenue += 10 * hiredMultiplier;
        revenueText.text = "Revenue:" + Revenue + " bn";

        if (!e.wasDecisionCorrect)
        {
            StrikesLeft--;
            strikesLeftText.text = "";

            for (int i = 0; i < StrikesLeft; i++)
                strikesLeftText.text += "I";
        }
    }

    IEnumerator FluctuateRevenue()
    {
        Revenue += Random.Range(-randomUpdateRange / 2, randomUpdateRange / 2);
        revenueText.text = "Revenue:" + Revenue + " bn";
        yield return new WaitForSeconds(randomUpdatePeriod);
        StartCoroutine(FluctuateRevenue());
    }

}
