using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class SlotMachineManager : MonoBehaviour
{
    [SerializeField] float timeFloor = .5f;
    [SerializeField] float timeCeiling = 3f;
    [SerializeField] float ceilingDivisor = 2;
    [SerializeField] List<Slot> slots;

    private void OnEnable()
        => DayManager.DayStateChangeEventHandler += HandleDayStateChange;

    private void OnDisable()
        => DayManager.DayStateChangeEventHandler -= HandleDayStateChange;

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayStateChangeEventArgs.DayState.StartWork:
                StartCoroutine(CO_StartSlots());
            break; 
        }
    }


    IEnumerator CO_StartSlots()
    {
        Debug.Log("Randomize Slots");
        yield return null;

        float randomTimeFloor = timeFloor;
        float randomTimeCeiling = timeCeiling;
        List<int> previousResults = new List<int>();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots.Count >= RestrictionHandler.Instance.Restrictions.Count)
                throw new Exception("Not enough restrictions");

            int random;
            do
                random = UnityEngine.Random.Range(0, slots.Count);
            while (previousResults.Contains(random));
            previousResults.Add(random);

            var result = RestrictionHandler.Instance.Restrictions[random];
            float randomTime = UnityEngine.Random.Range(randomTimeFloor, randomTimeCeiling);
            StartCoroutine(slots[i].CORandomize(randomTime, result));
            randomTimeFloor += randomTime;
            randomTimeCeiling += randomTimeCeiling / ceilingDivisor;
        }
    }
}
