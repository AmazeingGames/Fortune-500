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

    [SerializeField] private EventReference LoseGameEvent;
    [SerializeField] private EventReference GoodHireEvent;
    [SerializeField] private EventReference BadHireEvent;

    [Header("Pink Slip")]
    [SerializeField] Canvas slipParent;

    [Header("Prefabs")]
    [SerializeField] PinkSlip pinkSlip;
    public CandidateData CurrentCandidate { get; private set; }

    CandidateGenerator _candidateGenerator;
    ScoreKeeper _scoreKeeper;
    float _currentCandidatePatience;
    RestrictionHandler _restrictionHandler;
    int _candidatesInTheDay;
    bool enabled = true;
    [SerializeField] GameObject candidateToDisable;

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

    IEnumerator RestartGame()
    {
        yield return new WaitForSecondsRealtime(10f);
        SceneManager.LoadScene(0);
    }

    void MakeDesicion(bool wasHired)
    {
        if (!enabled) return;
        bool wasDesicionCorrect = wasHired == (RestrictionHandler.Instance.Restrictions[0].restriction(CurrentCandidate) && RestrictionHandler.Instance.Restrictions[1].restriction(CurrentCandidate) && RestrictionHandler.Instance.Restrictions[2].restriction(CurrentCandidate));
        
        if (!wasDesicionCorrect) { GeneratePinkSlip(CurrentCandidate, wasHired, RestrictionHandler.Instance.Restrictions);
            AudioManager.Instance.PlayOneShot(BadHireEvent, transform.position);
        }
        else AudioManager.Instance.PlayOneShot(GoodHireEvent, transform.position);
        if (_scoreKeeper.StrikesLeft == 1 && !wasDesicionCorrect)
        {
            _scoreKeeper.UpdateForCandidate(CurrentCandidate, wasDesicionCorrect);
            AudioManager.Instance.PlayOneShot(LoseGameEvent, transform.position);
            enabled = false;
            candidateToDisable.SetActive(false);
            StartCoroutine(RestartGame());
            return;
        }
        _scoreKeeper.UpdateForCandidate(CurrentCandidate, wasDesicionCorrect);
        GetNewCandidate();
    }

    PinkSlip slip = null;
    void GeneratePinkSlip(CandidateData candidate, bool wasHired, List<Restriction> restrictions)
    {
        string restrictionToDisplay;
        if (!restrictions[0].restriction(candidate))
            restrictionToDisplay = restrictions[0].description;
        else if (!restrictions[1].restriction(candidate))
            restrictionToDisplay = restrictions[1].description;
        else
            restrictionToDisplay = restrictions[2].description;

        string title = "WARNING";
        string mainText = "";
        string conclusion = "";
        if (wasHired)
        {
            mainText += $"You hired {candidate.FirstName} {candidate.LastName} , but he didn't comply with the following restriction: {Environment.NewLine}" +
                $"{restrictionToDisplay} {Environment.NewLine}. Are you questioning the Slot Machine's authority?";
        }

        else
        {
            mainText += $"You rejected a perfectly good employee, {candidate.FirstName} {candidate.LastName}. {Environment.NewLine}" +
                $"Who's gonna do all the important work we have around here? You?";
        }

        var localPosition = new Vector3(-358.809998f, -202.466995f, 7.09000015f);
        if (_scoreKeeper.StrikesLeft == 2)
        {
            conclusion = "Strike 1!";
        }
        else if (_scoreKeeper.StrikesLeft == 1)
        {
            conclusion = "Final warning!";
        }
        else if (_scoreKeeper.StrikesLeft == 0)
        {
            title = "TERMINATION NOTICE";
            conclusion = "GET OUT!";
        }

        
        if (slip == null)
            slip = Instantiate(pinkSlip, slipParent.transform);
        slip.transform.localPosition = localPosition;
        slip.Initialize(title, mainText, conclusion);
    }

    void GetNewCandidate()
    {
        /*if (_candidatesInTheDay == 0)
        {
            OnFinishCandidates();
            return;
        }*/

        

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