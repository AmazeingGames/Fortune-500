using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotMachineButton : MonoBehaviour, IPointerDownHandler
{
    public static EventHandler<SlotsInteractEventArgs> SlotsInteractEventHandler;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Start gambling!");

        SlotsInteractEventHandler?.Invoke(this, new(SlotsInteractEventArgs.InteractionType.PullLever));
    }
}

public class SlotsInteractEventArgs : EventArgs
{
    public enum InteractionType { PullLever }
    public readonly InteractionType myInteractionType;

    public SlotsInteractEventArgs(InteractionType myInteractionType)
    {
        this.myInteractionType = myInteractionType;
    }
}

