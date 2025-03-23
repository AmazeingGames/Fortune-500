using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotMachineButton : MonoBehaviour, IPointerDownHandler
{
    public static EventHandler PulledLever;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Start gambling!");

        PulledLever?.Invoke(this, new());
    }
}
