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
    {
        SlotMachineButton.PulledLever += HandlePullLever;
    }

    private void OnDisable()
    {
        SlotMachineButton.PulledLever -= HandlePullLever;
    }

    void HandlePullLever(object sender, EventArgs e)
        => StartCoroutine(COStartSlots());

    IEnumerator COStartSlots()
    {
        Debug.Log("Randomize Slots");
        yield return null;

        float randomTimeFloor = timeFloor;
        float randomTimeCeiling = timeCeiling;
        for (int i = 0; i < slots.Count; i++)
        {
            var result = RestrictionHandler.Instance.Restrictions[i];
            float randomTime = UnityEngine.Random.Range(randomTimeFloor, randomTimeCeiling);
            StartCoroutine(slots[i].CORandomize(randomTime, result));
            randomTimeFloor += randomTime;
            randomTimeCeiling += randomTimeCeiling / ceilingDivisor;
        }
    }
}
