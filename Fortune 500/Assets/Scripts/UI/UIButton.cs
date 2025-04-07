using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UIButton;

public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    [Header("Button Type")]
    [SerializeField] EventType myEventType;

    // Turn this into a class value, which inherits from the same type
    // Change the class based on the button type, and then serialize the class values
    // The script would have to run in the editor for this to work properly
    [Header("UI Button Type")]
    [SerializeField] UIManager.MenuType menuToOpen;

    [Header("Game State Button Type")]
    [SerializeField] GameFlowManager.GameAction newGameAction;

    [Header("Day State Button Type")]
    [SerializeField] DayManager.DayState newDayState;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image underline;

    public enum EventType { None, UI, GameAction, DayAction }
    public enum UIInteractionTypes { Enter, Click, Up, Exit }

    public static EventHandler<UIInteractEventArgs> UIInteractEventHandler;

    Coroutine lerpButtonCoroutine = null;
    Coroutine lerpUnderlineCoroutine = null;

    void OnEnable()
    {
        if (UIManager.Instance != null)
            StartCoroutine(LerpButton(false));
    }

    void OnDisable()
    {
        if (UIManager.Instance == null)
            return;

        text.transform.localScale = new (UIManager.Instance.RegularScale, UIManager.Instance.RegularScale);

        text.alpha = UIManager.Instance.RegularOpacity;

        underline.fillAmount = 0;
    }

    IEnumerator Start()
    {
        while (UIManager.Instance == null)
            yield return null;

        text.alpha = UIManager.Instance.RegularOpacity;
        text.gameObject.SetActive(true);

        var regularScale = UIManager.Instance.RegularScale;
        var rect = transform as RectTransform;

        text.transform.localScale = new Vector3(regularScale, regularScale, text.transform.localScale.z);
        underline.rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x, underline.rectTransform.sizeDelta.y);
        underline.fillAmount = 0;

        StartCoroutine(SetUnderlineLength());

        // Debug.Log($"Underline: size set to {underline.rectTransform.sizeDelta.x}");
    }

    IEnumerator SetUnderlineLength()
    {
        underline.rectTransform.sizeDelta = new Vector2(0, underline.rectTransform.sizeDelta.y);

        while (true)
        {
            var rect = transform as RectTransform;
            underline.rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x, underline.rectTransform.sizeDelta.y);
            yield return new WaitForSeconds(.1f);
        }
    }

    /// <summary> Moves the last held paper to the correct position over time. </summary>
    IEnumerator LerpButton(bool isSelected)
    {
        float time = 0;
        
        float startingScale = text.transform.localScale.x;
        float targetScale = isSelected ? UIManager.Instance.HoverScale : UIManager.Instance.RegularScale;

        float startingOpacity = text.alpha;
        float targetOpacity = isSelected ? UIManager.Instance.HoverOpacity : UIManager.Instance.RegularOpacity;

        while (time < 1)
        {
            var lerpCurve = UIManager.Instance.ButtonLerpCurve;
            
            float newScale = Mathf.Lerp(startingScale, targetScale, lerpCurve.Evaluate(time));
            text.transform.localScale = new Vector3 (newScale, newScale, text.transform.localScale.z);

            float newOpacity = Mathf.Lerp(startingOpacity, targetOpacity, lerpCurve.Evaluate(time));
            text.alpha = newOpacity;

            time += Time.deltaTime * UIManager.Instance.ButtonLerpSpeed;
            yield return null;
        }
    }

    IEnumerator LerpUnderline(bool isSelected)
    {
        float time = 0;

        float startingFill = underline.fillAmount;
        float targetFill = isSelected ? 1 : 0;

        while (time < 1)
        {
            var lerpCurve = UIManager.Instance.UnderlineLerpCurve;

            float newFillAmount = Mathf.Lerp(startingFill, targetFill, lerpCurve.Evaluate(time));
            underline.fillAmount = newFillAmount;

            time += Time.deltaTime * UIManager.Instance.UnderlineLerpSpeed;
            yield return null;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, UIInteractionTypes.Click);

        HighlightButton(false);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, UIInteractionTypes.Enter);

        HighlightButton(true);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, UIInteractionTypes.Exit);

        HighlightButton(false);
    }

    void HighlightButton(bool isSelected)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (lerpButtonCoroutine != null)
            StopCoroutine(lerpButtonCoroutine);

        lerpButtonCoroutine = StartCoroutine(LerpButton(isSelected));

        if (lerpUnderlineCoroutine != null)
            StopCoroutine(lerpUnderlineCoroutine);

        lerpUnderlineCoroutine = StartCoroutine(LerpUnderline(isSelected));
    }

    public void OnPointerUp(PointerEventData pointerEventData)
        => OnUIInteract(pointerEventData, UIInteractionTypes.Up);

    public virtual void OnUIInteract(PointerEventData pointerEventData, UIInteractionTypes buttonInteract)
        => UIInteractEventHandler?.Invoke(this, new(this, myEventType, pointerEventData, buttonInteract));


    public class UIInteractEventArgs : EventArgs
    {
        public readonly EventType buttonEvent;
        public readonly UIInteractionTypes buttonInteraction;
        public readonly PointerEventData pointerEventData;

        public readonly UIManager.MenuType menuToOpen = UIManager.MenuType.None;
        public readonly GameFlowManager.GameAction actionToPerform = GameFlowManager.GameAction.None;

        public readonly DayManager.DayState myNewDayState;

        public UIInteractEventArgs(UIButton button, EventType uiEventType, PointerEventData pointerEventData, UIInteractionTypes uiInteractionType)
        {
            this.buttonEvent = uiEventType;
            this.pointerEventData = pointerEventData;
            this.buttonInteraction = uiInteractionType;

            if (uiInteractionType == UIInteractionTypes.Enter || uiInteractionType == UIInteractionTypes.Exit)
                return;

            switch (uiEventType)
            {
                case EventType.UI:
                    menuToOpen = button.menuToOpen;

                    switch (menuToOpen)
                    {
                        case UIManager.MenuType.None:
                            throw new InvalidOperationException("A menu type of none will cause nothing to happen.");
                        case UIManager.MenuType.Pause:
                            throw new InvalidOperationException("Pausing the game should be done by updating the game state, not through changing UI menus.");
                        case UIManager.MenuType.Empty:
                            throw new InvalidOperationException("Closing all menus should be done by updating the game to the proper game state, not through changing UI menus.");
                    }
                break;

                case EventType.GameAction:
                    actionToPerform = button.newGameAction;

                    if (actionToPerform == GameFlowManager.GameAction.None)
                        throw new InvalidOperationException("A game state of none will cause nothing to happen.");
                break;

                case EventType.DayAction:
                    myNewDayState = button.newDayState;

                    if (myNewDayState != DayManager.DayState.StartDay)
                        throw new InvalidOperationException("A UI button should only change the day state to StartDay");
                break;
            }
        }
    }
}

