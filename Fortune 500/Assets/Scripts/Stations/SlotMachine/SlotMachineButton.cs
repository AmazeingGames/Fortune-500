using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotMachineButton : MonoBehaviour, IPointerDownHandler
{
    public static EventHandler PulledLeverEventHandler;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Start gambling!");

        PulledLeverEventHandler?.Invoke(this, new());
    }
}
