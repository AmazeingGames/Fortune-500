using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using static FocusStation;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions;

public class PinkSlip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI mistake;
    [SerializeField] TextMeshProUGUI conclusion;
    [SerializeField] GameObject slipVisuals;

    Vector3 mouseOffset;
    float mZCoord;

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mouseOffset = gameObject.transform.position - GetMouseWorldPosition();
    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mouseOffset;
    }

    private void OnEnable()
    {
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
        CandidateHandler.ReviewedCandidateEventHandler += HandleReviewedCandidate;
    }
    private void OnDisable()
    {
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
        CandidateHandler.ReviewedCandidateEventHandler -= HandleReviewedCandidate;
    }

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayManager.DayState.StartDay:
                slipVisuals.SetActive(false);
            break;

            case DayManager.DayState.StartWork:
            case DayManager.DayState.EndWork:
            case DayManager.DayState.EndDay:
            case DayManager.DayState.None:
            break;
        }
    }

    void HandleReviewedCandidate(object sender, ReviewedCandidateEventArgs e)
    {
        if (e.wasDecisionCorrect)
            return;

        slipVisuals.SetActive(true);

        Assert.IsNotNull(RestrictionHandler.Restrictions, "Restrictions list should not be null");
        Assert.IsFalse(e.candidateData == null, "Candidate data should not be null");

        CandidateData? candidateData = e.candidateData;
        List<RestrictionData> restrictionsData = RestrictionHandler.Restrictions;

        string restrictionToDisplay;
        if (!restrictionsData[0].restriction(candidateData))
            restrictionToDisplay = restrictionsData[0].description;
        else if (!restrictionsData[1].restriction(candidateData))
            restrictionToDisplay = restrictionsData[1].description;
        else
            restrictionToDisplay = restrictionsData[2].description;

        string title = "WARNING";
        string mainText = "";
        string conclusion = "";

        if (e.didHireCandidate)
        {
            mainText += $"You hired {candidateData.Value.FirstName} {candidateData.Value.LastName} , but he didn't comply with the following restriction: {Environment.NewLine}" +
                $"{restrictionToDisplay} {Environment.NewLine}. Are you questioning the Slot Machine's authority?";
        }

        else
        {
            mainText += $"You rejected a perfectly good employee, {candidateData.Value.FirstName} {candidateData.Value.LastName}. {Environment.NewLine}" +
                $"Who's gonna do all the important work we have around here? You?";
        }

        if (ScoreKeeper.StrikesLeft == 2)
            conclusion = "Strike 1!";
        else if (ScoreKeeper.StrikesLeft == 1)
            conclusion = "Final warning!";
        else if (ScoreKeeper.StrikesLeft == 0)
        {
            title = "TERMINATION NOTICE";
            conclusion = "GET OUT!";
        }

        this.title.text = title;
        mistake.text = mainText;
        this.conclusion.text = conclusion;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition; 
        mousePoint.z = mZCoord;

        return Camera.main.WorldToScreenPoint(mousePoint);
    }
}
