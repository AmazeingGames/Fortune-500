using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System.Collections;
using UnityEngine.SceneManagement;

public class CandidateHandler : MonoBehaviour
{
    [Header("Resume")]
    [SerializeField] Candidate candidate;
    [SerializeField] VirtualScreen resume;
    [SerializeField] Button _hireButton;
    [SerializeField] Button _rejectButton;
    [SerializeField] Slider _patienceSlider;
    [SerializeField] ResumeDisplay _resumeDisplay;

    [Header("Pink Slip")]
    [SerializeField] Canvas slipParent;

    [Header("Prefabs")]
    [SerializeField] PinkSlip pinkSlip;

    CandidateData? currentCandidateData;
    float _currentCandidatePatience;
    bool canMakeDecisions = true;

    public static EventHandler<ReviewedCandidateEventArgs> ReviewedCandidateEventHandler;

    private void Awake()
    {
        _hireButton.onClick.AddListener(() => MakeDecision(true));
        _rejectButton.onClick.AddListener(() => MakeDecision(false));
    }

    private void OnEnable()
    {
        GameFlowManager.PerformGameActionEventHandler += HandlePerformGameAction;

        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
    }

    private void OnDisable()
    {
        GameFlowManager.PerformGameActionEventHandler -= HandlePerformGameAction;

        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
    }

    private void Update()
        => UpdatePatience();

    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.LoseGame:
                currentCandidateData = null;
                candidate.gameObject.SetActive(false);
                resume.gameObject.SetActive(false);
            break;
        }
    }

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayManager.DayState.EndWork:
            case DayManager.DayState.StartDay:
                Debug.Log("set candidate data to null");
                currentCandidateData = null;
                candidate.gameObject.SetActive(false);
                resume.gameObject.SetActive(false);
            break;

            case DayManager.DayState.StartWork:
                canMakeDecisions = true;

                candidate.gameObject.SetActive(true);
                resume.gameObject.SetActive(true);
                GetNewCandidate();
            break;
        }
    }

    void MakeDecision(bool didHireCandidate)
    {
        if (!canMakeDecisions) 
            return;
        
        bool wasDecisionCorrect = didHireCandidate == (RestrictionHandler.Restrictions[0].restriction(currentCandidateData) && RestrictionHandler.Restrictions[1].restriction(currentCandidateData) && RestrictionHandler.Restrictions[2].restriction(currentCandidateData));
        
        if (DayManager.RemainingEmployees != 1 && ScoreKeeper.StrikesLeft != 1)
            GetNewCandidate();

        ReviewedCandidateEventHandler?.Invoke(this, new(wasDecisionCorrect, didHireCandidate, currentCandidateData));
    }

    void GetNewCandidate()
    {
        Debug.Log($"Generated new candidate: {DayManager.RemainingEmployees} employees remaining");
        currentCandidateData = CandidateGenerator.Instance.GenerateRandomCandidate();
        candidate.Init(currentCandidateData);

        _currentCandidatePatience = currentCandidateData.Value.Patience;
        _patienceSlider.maxValue = _currentCandidatePatience;
        _resumeDisplay.DisplayCandidate(currentCandidateData.Value);
    }   

    void UpdatePatience()
    {
        if (currentCandidateData == null)
            return;

        _currentCandidatePatience -= Time.deltaTime;
        _patienceSlider.value = _currentCandidatePatience;
        
        if (_currentCandidatePatience < 0) 
            MakeDecision(false);
    }
}

public class ReviewedCandidateEventArgs : EventArgs 
{
    public readonly bool wasDecisionCorrect;
    public readonly bool didHireCandidate;
    public readonly CandidateData? candidateData;
    public ReviewedCandidateEventArgs(bool wasDecisionCorrect, bool didHireCandidate, CandidateData? candidateData) 
    { 
        this.wasDecisionCorrect = wasDecisionCorrect;
        this.didHireCandidate = didHireCandidate;
        this.candidateData = candidateData;
    }
}