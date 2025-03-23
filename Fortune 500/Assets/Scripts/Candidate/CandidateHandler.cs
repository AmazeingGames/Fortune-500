using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CandidateHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _restrictionText;
    [SerializeField] Button _hireButton;
    [SerializeField] Button _rejectButton;
    [SerializeField] Slider _patienceSlider;

    public CandidateData CurrentCandidate { get; private set; }

    CandidateGenerator _candidateGenerator;
    ScoreKeeper _scoreKeeper;
    float _currentCandidatePatience;
    ResumeDisplay _resumeDisplay;
    RestrictionHandler _restrictionHandler;
    int _candidatesInTheDay;

    List<Tuple<string, Func<CandidateData, bool>>> _restrictions;

    private void Awake()
    {
        _resumeDisplay = FindAnyObjectByType<ResumeDisplay>();
        _scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
        _candidateGenerator = FindAnyObjectByType<CandidateGenerator>();
        _restrictionHandler = FindAnyObjectByType<RestrictionHandler>();
        _hireButton.onClick.AddListener(()=> MakeDesition(true));
        _rejectButton.onClick.AddListener(() => MakeDesition(false));
    }

    private void Start()
    {
        StartNewDay();
    }

    void MakeDesition(bool wasHired)
    {
        bool wasDesitionCorrect = wasHired == (_restrictions[0].Item2(CurrentCandidate) && _restrictions[1].Item2(CurrentCandidate) &&
             _restrictions[2].Item2(CurrentCandidate));
        _scoreKeeper.UpdateForCandidate(CurrentCandidate, wasDesitionCorrect);
        GetNewCandidate();
    }


    void GetNewCandidate()
    {
        if (_candidatesInTheDay == 0)
        {
            StartNewDay();
            return;
        }
        _candidatesInTheDay--;
        CurrentCandidate = _candidateGenerator.GenerateRandomCandidate();
        _currentCandidatePatience = CurrentCandidate.Patience;
        _patienceSlider.maxValue = _currentCandidatePatience;
        _resumeDisplay.DisplayCandidate(CurrentCandidate);
    }

    private void Update()
    {
        _currentCandidatePatience -= Time.deltaTime;
        _patienceSlider.value = _currentCandidatePatience;
        if (_currentCandidatePatience < 0) MakeDesition(false);
    }

    private void StartNewDay()
    {
        _candidatesInTheDay = 5;
        _scoreKeeper.StartNewDay();
        _restrictions = _restrictionHandler.GenerateRestrictions();
        _restrictionText.text = _restrictions[0].Item1 + Environment.NewLine + _restrictions[1].Item1
            + Environment.NewLine + _restrictions[2].Item1;
        GetNewCandidate();
    }

}
