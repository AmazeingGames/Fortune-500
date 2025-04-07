using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using static DayManager;

public class SlotMachineManager : MonoBehaviour
{
    [SerializeField] int randomSpinsFloor = 3;
    [SerializeField] int randomSpinsCeiling = 5;
    [SerializeField] List<Slot> slots;
    [SerializeField] float delayBetweenSlots = .5f;

    int finishedSlots;

    public static EventHandler<RandomizeSlotsEventArgs> RandomizedSlotsEventHandler;

    private void OnEnable()
    {
        SlotMachineButton.SlotsInteractEventHandler += HandlePullLever;
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
    }


    private void OnDisable()
    {
        SlotMachineButton.SlotsInteractEventHandler -= HandlePullLever;
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
    }

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayState.StartDay:
                finishedSlots = 0;
            break;
            
            case DayState.None:
            case DayState.StartWork:
            case DayState.EndWork:
            case DayState.EndDay:
            break;
        }
    }


    void HandlePullLever(object sender, SlotsInteractEventArgs e)
    {
        switch (e.myInteractionType)
        {
            case SlotsInteractEventArgs.InteractionType.PullLever:
                StartCoroutine(CO_StartSlots());
            break;
        }
    }

    IEnumerator CO_StartSlots()
    {
        yield return null;

        List<int> previousResults = new List<int>();
        int timesToSpin = UnityEngine.Random.Range(randomSpinsFloor, randomSpinsCeiling);
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots.Count >= RestrictionHandler.Restrictions.Count)
                throw new Exception("Not enough restrictions");

            int random;
            do
                random = UnityEngine.Random.Range(0, slots.Count);
            while (previousResults.Contains(random));
            
            previousResults.Add(random);

            var result = RestrictionHandler.Restrictions[random];
            slots[i].Spin(timesToSpin, result, this);
            timesToSpin++;

            yield return new WaitForSeconds(delayBetweenSlots);
        }
    }

    public void SlotFinished()
    {
        finishedSlots++;

        if (finishedSlots == slots.Count)
            RandomizedSlotsEventHandler?.Invoke(this, new());
    }
}

public class RandomizeSlotsEventArgs : EventArgs
{

}
