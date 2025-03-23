using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] int _randomUpdateRange;
    [SerializeField] float _randomUpdatePeriod;

    [SerializeField] TextMeshProUGUI _revenueText;
    [SerializeField] TextMeshProUGUI _strikesLeftText;

    public int Revenue { get; private set; }
    public int StrikesLeft { get; private set; } = 3;

    void Start()
    {
        Revenue = 100;
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

    public void UpdateForCandidate(Candidate candidate, bool wasChoiceCorrect)
    {
        int hiredMultiplier = wasChoiceCorrect ? 1 : -1;
        Revenue += 10 * hiredMultiplier ;
        _revenueText.text = "Revenue:" + Revenue + " bn";
    }
}
