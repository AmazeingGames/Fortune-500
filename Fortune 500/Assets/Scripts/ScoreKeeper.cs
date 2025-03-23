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
    [SerializeField] TextMeshProUGUI _reputationText;

    public int Revenue { get; private set; }
    public int Reputation { get; private set; }

    void Start()
    {
        Revenue = 100;
        Reputation = 100;
        StartCoroutine(Fluctuate());
    }

    private void Update()
    {
        _revenueText.text = "Revenue:" + Revenue + " bn";
        _reputationText.text = "Reputation:" + Reputation;
    }

    IEnumerator Fluctuate()
    {
        Revenue += Random.Range(-_randomUpdateRange / 2, _randomUpdateRange / 2);
        Reputation += Random.Range(-_randomUpdateRange / 2, _randomUpdateRange / 2);
        yield return new WaitForSeconds(_randomUpdatePeriod);
        
        StartCoroutine(Fluctuate());
    }

    /*public void UpdateForCandidate(Candidate candidate, bool wasHired)
    {
        int hiredMultiplier = wasHired ? 1 : -1;

        Revenue += 10 * hiredMultiplier;
        Reputation -= 10 * hiredMultiplier;
    }*/

}
