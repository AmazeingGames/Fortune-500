using System;
using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;

public class Slot : MonoBehaviour
{
    [SerializeField] int timeForEachFrame = 10;
    [SerializeField] TextMeshProUGUI restrictionResult;

    public IEnumerator CORandomize(float timeToSpinInSeconds, Restriction result)
    {
        Debug.Log("Randomize!");
        /*
        float timerInSeconds = 0;
        float frames = 0;
        
        while (timerInSeconds < timeToSpinInSeconds)
        {
            frames++;
            yield return null;

            if (frames >= timeForEachFrame)
            {
                frames = 0;
                RandomizeOption();
            }
        }

        
        yield return new WaitForEndOfFrame();
        */
        restrictionResult.text = $"{result.myRestrictionType}";

        yield break;
    }

    void RandomizeOption()
    {
        /*
        Debug.Log("random");
        var listOfEnums = Enum.GetValues(typeof(RestrictionHandler.RestrictionType)).Cast<RestrictionHandler.RestrictionType>().ToList();

        RestrictionHandler.RestrictionType randomEnum;
        int times = 0;
        do
        {
            times++;
            randomEnum = (RestrictionHandler.RestrictionType)UnityEngine.Random.Range(0, listOfEnums.Count);
        }
        while (restrictionResult.text != $"{randomEnum}" || times < 20);

        restrictionResult.text = randomEnum.ToString();
        */
    }
}
