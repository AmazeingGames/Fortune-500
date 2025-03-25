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
        => GameFlowManager.GameActionEventHandler += HandleGameAction;

    private void OnDisable()
        => GameFlowManager.GameActionEventHandler -= HandleGameAction;

    void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.StartWork:
                StartCoroutine(COStartSlots());
                break;
        }
    }


    IEnumerator COStartSlots()
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
