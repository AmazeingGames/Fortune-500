using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotMachineButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameObject lever;
    [SerializeField] Vector3 targetRotation;
    [SerializeField] Vector3 startingRotation;
    [SerializeField] float downDuration;
    [SerializeField] float upDuration;

    public static EventHandler<SlotsInteractEventArgs> SlotsInteractEventHandler;

    Image image;
    Button button;
    TMPro.TextMeshProUGUI text;

    bool hasRandomizedToday;
    bool allowMultipleRandomizations;

    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();

        lever.transform.DOLocalRotate(startingRotation, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("Start gambling!");
        if (!PlayerFocus.IsFocusedOn(PlayerFocus.Station.Slots))
        {
            image.enabled = false;
            button.enabled = false;
            return;
        }

        bool isEditor = false;
#if UNITY_EDITOR
        isEditor = true;
#endif
        if (!hasRandomizedToday || (allowMultipleRandomizations && isEditor))
        {
            if (allowMultipleRandomizations && isEditor)
            {
                hasRandomizedToday = true;
                image.enabled = false;
                button.enabled = false;
                text.gameObject.SetActive(false);
            }
            
            SlotsInteractEventHandler?.Invoke(this, new(SlotsInteractEventArgs.InteractionType.PullLever));
            lever.transform.DOLocalRotate(targetRotation, downDuration);
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

            case DayManager.DayState.StartWork:
                lever.transform.DOLocalRotate(startingRotation, upDuration);
                break;

            case DayManager.DayState.None:
            case DayManager.DayState.EndWork:
            case DayManager.DayState.EndDay:
            break;
        }

    }
}

public class SlotsInteractEventArgs : EventArgs
{
    public enum InteractionType { None, PullLever, GetResults }
    public readonly InteractionType myInteractionType;

    public SlotsInteractEventArgs(InteractionType myInteractionType)
    {
        this.myInteractionType = myInteractionType;
    }
}

