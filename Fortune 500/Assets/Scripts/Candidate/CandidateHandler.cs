using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CandidateHandler : MonoBehaviour
{
    [SerializeField] Candidate candidate;
    [SerializeField] VirtualScreen resume;
    [SerializeField] TextMeshPro _restrictionText;
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

    List<Restriction> _restrictions;

    public static EventHandler<FinishedCandidatesEventArgs> FinishedCandidatesEventHandler;

    private void Awake()
    {
        _resumeDisplay = FindAnyObjectByType<ResumeDisplay>();
        _scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
        _candidateGenerator = FindAnyObjectByType<CandidateGenerator>();
        _restrictionHandler = FindAnyObjectByType<RestrictionHandler>();

        _hireButton.onClick.AddListener(()=> MakeDesicion(true));
        _rejectButton.onClick.AddListener(() => MakeDesicion(false));
    }

    private void OnEnable()
        => GameManager.GameActionEventHandler += HandleGameAction;

    private void OnDisable()
        => GameManager.GameActionEventHandler += HandleGameAction;

    private void Update()
        => UpdatePatience();

    void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameManager.GameAction.StartDay:
                candidate.gameObject.SetActive(false);
                resume.gameObject.SetActive(false);
            break;

            case GameManager.GameAction.StartWork:
                candidate.gameObject.SetActive(true);
                resume.gameObject.SetActive(true);

                _candidatesInTheDay = 5;
                _scoreKeeper.StartNewDay();
                _restrictions = _restrictionHandler.GenerateRestrictions();
                _restrictionText.text = _restrictions[0].description + Environment.NewLine + _restrictions[1].description + Environment.NewLine + _restrictions[2].description;
                GetNewCandidate();
            break;
        }
    }

    void MakeDesicion(bool wasHired)
    {
        bool wasDesicionCorrect = wasHired == (_restrictions[0].restriction(CurrentCandidate) && _restrictions[1].restriction(CurrentCandidate) && _restrictions[2].restriction(CurrentCandidate));
        _scoreKeeper.UpdateForCandidate(CurrentCandidate, wasDesicionCorrect);
        GetNewCandidate();
    }

    void GetNewCandidate()
    {
        if (_candidatesInTheDay == 0)
        {
            OnFinishCandidates();
            return;
        }

        _candidatesInTheDay--;
        CurrentCandidate = _candidateGenerator.GenerateRandomCandidate();
        candidate.Init(CurrentCandidate);

        _currentCandidatePatience = CurrentCandidate.Patience;
        _patienceSlider.maxValue = _currentCandidatePatience;
        _resumeDisplay.DisplayCandidate(CurrentCandidate);
    }

    void OnFinishCandidates()
        => FinishedCandidatesEventHandler?.Invoke(this, new());

    void UpdatePatience()
    {
        if (CurrentCandidate == null)
            return;

        _currentCandidatePatience -= Time.deltaTime;
        _patienceSlider.value = _currentCandidatePatience;
        
        if (_currentCandidatePatience < 0) 
            MakeDesicion(false);
    }
}

public class FinishedCandidatesEventArgs : EventArgs { }