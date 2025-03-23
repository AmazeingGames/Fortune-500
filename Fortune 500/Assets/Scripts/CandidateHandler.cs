using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandidateHandler : MonoBehaviour
{
    [SerializeField] Button _hireButton;
    [SerializeField] Button _rejectButton;
    [SerializeField] Slider _patienceSlider;

    public Candidate CurrentCandidate { get; private set; }
    public List<Candidate> Employees { get; private set; } = new List<Candidate>();

    CandidateGenerator _candidateGenerator;
    ScoreKeeper _scoreKeeper;
    float _currentCandidatePatience;
    ResumeDisplay _resumeDisplay;

    private void Awake()
    {
        _resumeDisplay = FindAnyObjectByType<ResumeDisplay>();
        _scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
        _candidateGenerator = FindAnyObjectByType<CandidateGenerator>();
        _hireButton.onClick.AddListener(Hire);
        _rejectButton.onClick.AddListener(Reject);
    }

    private void Start()
    {
        GetNewCandidate();
    }

    void Hire()
    {
        Employees.Add(CurrentCandidate);
        _scoreKeeper.UpdateForCandidate(CurrentCandidate, true);
        GetNewCandidate();
    }

    void Reject()
    {
        _scoreKeeper.UpdateForCandidate(CurrentCandidate, false);
        GetNewCandidate();
    }

    void GetNewCandidate()
    {
        CurrentCandidate = _candidateGenerator.GenerateRandomCandidate();
        _currentCandidatePatience = CurrentCandidate.Patience;
        _patienceSlider.maxValue = _currentCandidatePatience;
        _resumeDisplay.DisplayCandidate(CurrentCandidate);
    }

    private void Update()
    {
        _currentCandidatePatience -= Time.deltaTime;
        _patienceSlider.value = _currentCandidatePatience;
        if (_currentCandidatePatience < 0) Reject();
    }

}
