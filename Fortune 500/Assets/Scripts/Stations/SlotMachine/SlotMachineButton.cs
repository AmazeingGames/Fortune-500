using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotMachineButton : MonoBehaviour, IPointerDownHandler
{
    public static EventHandler<SlotsInteractEventArgs> SlotsInteractEventHandler;

    Image image;
    Button button;
    TMPro.TextMeshProUGUI text;

    bool hasRandomizedToday;

    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>(); 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("Start gambling!");

        if (!hasRandomizedToday)
        {
            hasRandomizedToday = true;
            image.enabled = false;
            button.enabled = false;
            text.gameObject.SetActive(false);
            SlotsInteractEventHandler?.Invoke(this, new(SlotsInteractEventArgs.InteractionType.PullLever));
        }
    }

    private void OnEnable()
    {
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
    }

    private void OnDisable()
    {
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
    }

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayManager.DayState.StartDay:
                hasRandomizedToday = false;
                image.enabled = true;
                button.enabled = true;
                text.gameObject.SetActive(true);
            break;

            case DayManager.DayState.None:
            case DayManager.DayState.StartWork:
            case DayManager.DayState.EndWork:
            case DayManager.DayState.EndDay:
            break;
        }

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

