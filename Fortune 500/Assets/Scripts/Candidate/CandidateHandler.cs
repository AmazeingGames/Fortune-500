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
    [SerializeField] ResumeDisplay _resumeDisplay;

    public CandidateData CurrentCandidate { get; private set; }

    CandidateGenerator _candidateGenerator;
    ScoreKeeper _scoreKeeper;
    float _currentCandidatePatience;
    RestrictionHandler _restrictionHandler;
    int _candidatesInTheDay;

    public static EventHandler<FinishedCandidatesEventArgs> FinishedCandidatesEventHandler;

    private void Awake()
    {
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
                _restrictionText.text = RestrictionHandler.Instance.Restrictions[0].description + Environment.NewLine + RestrictionHandler.Instance.Restrictions[1].description + Environment.NewLine + RestrictionHandler.Instance.Restrictions[2].description;
                GetNewCandidate();
            break;
        }
    }

    void MakeDesicion(bool wasHired)
    {
        bool wasDesicionCorrect = wasHired == (RestrictionHandler.Instance.Restrictions[0].restriction(CurrentCandidate) && RestrictionHandler.Instance.Restrictions[1].restriction(CurrentCandidate) && RestrictionHandler.Instance.Restrictions[2].restriction(CurrentCandidate));
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