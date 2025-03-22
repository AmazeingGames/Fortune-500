using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandidateHandler : MonoBehaviour
{
    [SerializeField] Button _hireButton;
    [SerializeField] Button _rejectButton;

    public Candidate CurrentCandidate { get; private set; }
    public List<Candidate> Employees { get; private set; } = new List<Candidate>();

    CandidateGenerator _candidateGenerator;
    ScoreKeeper _scoreKeeper;


    private void Awake()
    {
        _scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
        _candidateGenerator = FindAnyObjectByType<CandidateGenerator>();
        _hireButton.onClick.AddListener(Hire);
        _rejectButton.onClick.AddListener(Reject);

    }

    private void Start()
    {
        CurrentCandidate = _candidateGenerator.GenerateRandomCandidate(Random.Range(0, 2));
    }

    void Hire()
    {
        Employees.Add(CurrentCandidate);
        _scoreKeeper.UpdateForCandidate(CurrentCandidate, true);
        CurrentCandidate = _candidateGenerator.GenerateRandomCandidate(Random.Range(0,2));
    }

    void Reject()
    {
        _scoreKeeper.UpdateForCandidate(CurrentCandidate, false);
        CurrentCandidate = _candidateGenerator.GenerateRandomCandidate(Random.Range(0, 2));
    }


}
