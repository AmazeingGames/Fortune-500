using System;
using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Slot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI result_TMP;
    [SerializeField] Image icon;
    [SerializeField] Vector2 topPosition;
    [SerializeField] Vector2 bottomPosition;
    [SerializeField] Vector2 centerPosition;
    [SerializeField] float tweenDuration;
    private void Start()
    {
        RandomizeSlot();
    }

    public async void Spin(int numberOfSpins, RestrictionData results, SlotMachineManager slotMachineManager)
    {
        Debug.Log($"Slot started spinning: {numberOfSpins} times");
        var tasks = new List<Task>();

        await icon.rectTransform.DOLocalMove(bottomPosition, tweenDuration / 2).AsyncWaitForCompletion();

        for (int i = 0; i < numberOfSpins; i++)
        {
            Debug.Log("task finished");
            icon.rectTransform.localPosition = topPosition;
            RandomizeSlot();
            
            await icon.rectTransform.DOLocalMove(bottomPosition, tweenDuration).AsyncWaitForCompletion();
        }

        await Task.WhenAll(tasks);

        icon.rectTransform.localPosition = topPosition;
        result_TMP.text = $"{results.myRestrictionType}";
        icon.sprite = results.icon;
        
        await icon.rectTransform.DOLocalMove(centerPosition, tweenDuration / 2).AsyncWaitForCompletion();

        slotMachineManager.SlotFinished();
    }


    void RandomizeSlot()
    {
        Debug.Log("randomized");
        var listOfEnums = Enum.GetValues(typeof(RestrictionHandler.RestrictionType)).Cast<RestrictionHandler.RestrictionType>().ToList();

        result_TMP.text = listOfEnums.SelectRandomElement().ToString();
        icon.sprite = RestrictionHandler.icons.SelectRandomElement();
    }
}

/*void StartSpin(int numberOfSpins)
{
    var disappearSequence = DOTween.Sequence();

    disappearSequence.Append(icon.transform.DOLocalMove(bottomPosition, tweenDuration / 2));
    for (int i = 0; i < numberOfSpins; i++)
    {
        disappearSequence.Append(icon.transform.DOLocalMove(topPosition, 0));
        disappearSequence.Append(icon.transform.DOLocalMove(bottomPosition, tweenDuration));
    }
    disappearSequence.Append(icon.transform.DOLocalMove(centerPosition, tweenDuration / 2));

    disappearSequence.OnComplete(() =>
    {

    });
}*/