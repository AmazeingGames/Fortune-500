using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] int _randomUpdateRange;
    [SerializeField] float _randomUpdatePeriod;

    [SerializeField] TextMeshPro _revenueText;
    [SerializeField] TextMeshPro _strikesLeftText;
    [SerializeField] TextMeshPro _dayText;

    public int Revenue { get; private set; }
    public int StrikesLeft { get; private set; } = 3;
    public int DayCount { get; private set; } = 0;

    void Start()
    {
        Revenue = 100;
        _strikesLeftText.text = "III";
        StartCoroutine(FluctuateRevenue());
    }

    private void Update()
    {
        _revenueText.text = "Revenue:" + Revenue + " bn";
    }

    IEnumerator FluctuateRevenue()
    {
        Revenue += Random.Range(-_randomUpdateRange / 2, _randomUpdateRange / 2);
        _revenueText.text = "Revenue:" + Revenue + " bn";
        yield return new WaitForSeconds(_randomUpdatePeriod);
        StartCoroutine(FluctuateRevenue());
    }

    public void UpdateForCandidate(CandidateData candidate, bool wasChoiceCorrect)
    {
        int hiredMultiplier = wasChoiceCorrect ? 1 : 0;
        Revenue += 10 * hiredMultiplier ;
        _revenueText.text = "Revenue:" + Revenue + " bn";
        if (!wasChoiceCorrect)
        {
            StrikesLeft--;
            _strikesLeftText.text = "";
            
            for (int i = 0; i < StrikesLeft; i++)
                _strikesLeftText.text += "I";
        }
            
    }

    public void StartNewDay()
    {
        DayCount++;
        _dayText.text = $"{DayCount}";
    }
}
