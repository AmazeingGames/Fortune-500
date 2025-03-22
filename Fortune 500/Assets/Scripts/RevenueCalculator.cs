using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevenueCalculator : MonoBehaviour
{
    [SerializeField] int _randomUpdateRange;
    [SerializeField] float _randomUpdatePeriod;

    [SerializeField] Button _correctButton;
    [SerializeField] Button _wrongButton;
    [SerializeField] TextMeshProUGUI _revenueText;

    public int Revenue { get; private set; }

    void Start()
    {
        Revenue = 100;
        StartCoroutine(Fluctuate());
    }

    private void Update()
    {
        _revenueText.text = "Revenue:" + Revenue + " bn";


    }

    IEnumerator Fluctuate()
    {
        Revenue += Random.Range(-_randomUpdateRange / 2, _randomUpdateRange / 2);
        yield return new WaitForSeconds(_randomUpdatePeriod);
        StartCoroutine(Fluctuate());
    }
}
