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
    [SerializeField] ButtonType myButtonType;

    // Turn this into a class value, which inherits from the same type
    // Change the class based on the button type, and then serialize the class values
    // The script would have to run in the editor for this to work properly
    [Header("UI Button Type")]
    [SerializeField] UIManager.MenuType myMenuToOpen;

    [Header("Game State Button Type")]
    [SerializeField] GameFlowManager.GameAction myGameActionToPerform;

    [Header("Day State Button Type")]
    [SerializeField] DayManager.DayState myNewDayState;

    [Header("Linked Setting")]
    [SerializeField] Settings.LinkedSetting myLinkedSetting;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI text_TMP;
    [SerializeField] Image underline;

    public enum ButtonType { None, UI, GameAction, DayAction, Setting }
    public enum UIInteractionTypes { Enter, Click, Up, Exit }

    public static EventHandler<UIInteractEventArgs> UIInteractEventHandler;

    Coroutine lerpButtonCoroutine = null;
    Coroutine lerpUnderlineCoroutine = null;

    IEnumerator Start()
    {
        // For some reason having a yield statement makes sure the underline displays properly
        while (UIManager.Instance == null)
            yield return null;

        text_TMP.alpha = UIManager.Instance.RegularOpacity;
        text_TMP.gameObject.SetActive(true);

        var regularScale = UIManager.Instance.RegularScale;
        var rectTransform = transform as RectTransform;

        text_TMP.transform.localScale = new Vector3(regularScale, regularScale, text_TMP.transform.localScale.z);
        underline.rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, underline.rectTransform.sizeDelta.y);
        underline.fillAmount = 0;

        StartCoroutine(SetUnderlineLength());

        // Debug.Log($"Underline: size set to {underline.rectTransform.sizeDelta.x}");

        HandleUpdateSettings(null, new(myLinkedSetting));
    }

    void OnEnable()
    {
        Settings.UpdateSettingsEventHandler += HandleUpdateSettings;

        if (UIManager.Instance != null)
            StartCoroutine(LerpButton(false));
    }

    void OnDisable()
    {
        Settings.UpdateSettingsEventHandler -= HandleUpdateSettings;

        if (UIManager.Instance == null)
            return;

        text_TMP.transform.localScale = new (UIManager.Instance.RegularScale, UIManager.Instance.RegularScale);
        text_TMP.alpha = UIManager.Instance.RegularOpacity;
        underline.fillAmount = 0;

    }

    void HandleUpdateSettings(object sender, UpdateSettingsEventArgs e)
    {
        switch (myButtonType)
        {
            case ButtonType.Setting:
                text_TMP.text = Settings.LinkedSettingToSetting[myLinkedSetting].ToString().FirstCharToUpper();
            break;

            case ButtonType.None:
            case ButtonType.UI:
            case ButtonType.GameAction:
            case ButtonType.DayAction:
            break;
        }
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
        
        float startingScale = text_TMP.transform.localScale.x;
        float targetScale = isSelected ? UIManager.Instance.HoverScale : UIManager.Instance.RegularScale;

        float startingOpacity = text_TMP.alpha;
        float targetOpacity = isSelected ? UIManager.Instance.HoverOpacity : UIManager.Instance.RegularOpacity;

        while (time < 1)
        {
            var lerpCurve = UIManager.Instance.ButtonLerpCurve;
            
            float newScale = Mathf.Lerp(startingScale, targetScale, lerpCurve.Evaluate(time));
            text_TMP.transform.localScale = new Vector3 (newScale, newScale, text_TMP.transform.localScale.z);

            float newOpacity = Mathf.Lerp(startingOpacity, targetOpacity, lerpCurve.Evaluate(time));
            text_TMP.alpha = newOpacity;

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
        => UIInteractEventHandler?.Invoke(this, new(this, myButtonType, pointerEventData, buttonInteract));

    public class UIInteractEventArgs : EventArgs
    {
        public readonly ButtonType myButtonType;
        public readonly UIInteractionTypes myInteractionType;
        public readonly PointerEventData pointerEventData;

        public readonly UIManager.MenuType myMenuToOpen = UIManager.MenuType.None;
        public readonly GameFlowManager.GameAction myActionToPerform = GameFlowManager.GameAction.None;
        public readonly DayManager.DayState myNewDayState;
        public readonly Settings.LinkedSetting myLinkedSetting;

        public UIInteractEventArgs(UIButton button, ButtonType buttonType, PointerEventData pointerEventData, UIInteractionTypes uiInteractionType)
        {
            this.myButtonType = buttonType;
            this.pointerEventData = pointerEventData;
            this.myInteractionType = uiInteractionType;

            if (uiInteractionType == UIInteractionTypes.Enter || uiInteractionType == UIInteractionTypes.Exit)
                return;

            switch (buttonType)
            {
                case ButtonType.UI:
                    myMenuToOpen = button.myMenuToOpen;

                    switch (myMenuToOpen)
                    {
                        case UIManager.MenuType.None:
                            throw new InvalidOperationException("A menu type of none will cause nothing to happen.");
                        case UIManager.MenuType.Pause:
                            throw new InvalidOperationException("Pausing the game should be done by updating the game state, not through changing UI menus.");
                        case UIManager.MenuType.Empty:
                            throw new InvalidOperationException("Closing all menus should be done by updating the game to the proper game state, not through changing UI menus.");
                    }
                break;

                case ButtonType.GameAction:
                    myActionToPerform = button.myGameActionToPerform;

                    if (myActionToPerform == GameFlowManager.GameAction.None)
                        throw new InvalidOperationException("A game state of none will cause nothing to happen.");
                break;

                case ButtonType.DayAction:
                    myNewDayState = button.myNewDayState;

                    if (myNewDayState != DayManager.DayState.StartDay)
                        throw new InvalidOperationException("A UI button should only change the day state to StartDay");
                break;

                case ButtonType.Setting:
                    myLinkedSetting = button.myLinkedSetting;
                break;
            }
        }
    }
}

