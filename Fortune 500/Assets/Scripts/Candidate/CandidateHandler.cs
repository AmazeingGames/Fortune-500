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
    [SerializeField] TextMeshPro _restrictionText;
    [SerializeField] Button _hireButton;
    [SerializeField] Button _rejectButton;
    [SerializeField] Slider _patienceSlider;
    [SerializeField] ResumeDisplay _resumeDisplay;

    [Header("Pink Slip")]
    [SerializeField] Canvas slipParent;

    [Header("Prefabs")]
    [SerializeField] PinkSlip pinkSlip;
    public static CandidateData CurrentCandidateData { get; private set; }
    float _currentCandidatePatience;
    int _candidatesInTheDay;
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
                CurrentCandidateData = null;
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
                CurrentCandidateData = null;
                candidate.gameObject.SetActive(false);
                resume.gameObject.SetActive(false);
            break;

            case DayManager.DayState.StartWork:
                canMakeDecisions = true;

                candidate.gameObject.SetActive(true);
                resume.gameObject.SetActive(true);

                _candidatesInTheDay = 5;
                _restrictionText.text = RestrictionHandler.Restrictions[0].description + Environment.NewLine + RestrictionHandler.Restrictions[1].description + Environment.NewLine + RestrictionHandler.Restrictions[2].description;
                GetNewCandidate();
            break;
        }
    }

    void MakeDecision(bool didHireCandidate)
    {
        if (!canMakeDecisions) 
            return;
        
        bool wasDecisionCorrect = didHireCandidate == (RestrictionHandler.Restrictions[0].restriction(CurrentCandidateData) && RestrictionHandler.Restrictions[1].restriction(CurrentCandidateData) && RestrictionHandler.Restrictions[2].restriction(CurrentCandidateData));
        
        ReviewedCandidateEventHandler?.Invoke(this, new(wasDecisionCorrect, ScoreKeeper.Instance.StrikesLeft, didHireCandidate));
        GetNewCandidate();
    }
    void GetNewCandidate()
    {
        _candidatesInTheDay--;
        CurrentCandidateData = CandidateGenerator.Instance.GenerateRandomCandidate();
        candidate.Init(CurrentCandidateData);

        _currentCandidatePatience = CurrentCandidateData.Patience;
        _patienceSlider.maxValue = _currentCandidatePatience;
        _resumeDisplay.DisplayCandidate(CurrentCandidateData);
    }   

    void UpdatePatience()
    {
        if (CurrentCandidateData == null)
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
    public readonly int strikesRemaining;
    public readonly bool didHireCandidate;
    public bool DidLose => strikesRemaining == 0;

    public ReviewedCandidateEventArgs(bool wasDecisionCorrect, int strikesRemaining, bool didHireCandidate) 
    { 
        this.wasDecisionCorrect = wasDecisionCorrect;
        this.strikesRemaining = strikesRemaining;
        this.didHireCandidate = didHireCandidate;
    }
}