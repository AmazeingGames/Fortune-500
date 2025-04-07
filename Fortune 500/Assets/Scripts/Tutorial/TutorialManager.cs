using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] float playAppearDelay = 5;
    [SerializeField] float playDisappearDelay = 5;
    [SerializeField] float globalDisappearDelay = 3;


    [Header("Components")]
    [SerializeField] TMPro.TextMeshProUGUI tutorialText_TMP;
    [SerializeField] GameObject background;
    // [SerializeField] ViewCheck requirementsViewCheck;

    enum TutorialTextType { None, PlaySlots, HireRequirements, ComputerEnter, ResumeEnter, SlotMachineEnter, PlayGame }

    TutorialTextType myCurrentTutorialText;

    readonly List<TutorialTextType> displayedTips = new();

    private void OnEnable()
    {
        GameFlowManager.PerformGameActionEventHandler += HandlePerformGameAction;
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnected;
    }
    private void OnDisable()
    {
        GameFlowManager.PerformGameActionEventHandler -= HandlePerformGameAction;
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnected;
    }

    void HandleInterfaceConnected(object sender, InterfaceConnectedEventArgs e)
    {
        switch (e.myStation)
        {
            case PlayerFocus.Station.Nothing:
                break;
            case PlayerFocus.Station.Computer:
                if (e.myInteractionType == FocusStation.InteractionType.Connect)
                    UpdateTutorialText("Feature coming soon!", TutorialTextType.ComputerEnter, 0, 3);
                break;

            case PlayerFocus.Station.Resume:
                if (e.myInteractionType == FocusStation.InteractionType.Connect)
                    UpdateTutorialText("Make sure to double-check the hiring requirements before onboarding any potential employees.", TutorialTextType.ResumeEnter, 0, 5);
                break;

            case PlayerFocus.Station.Slots:
                if (e.myInteractionType == FocusStation.InteractionType.Connect)
                UpdateTutorialText("Pull the lever to start your work day.", TutorialTextType.SlotMachineEnter, 0, globalDisappearDelay);
                break;
        }
    }



    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayManager.DayState.None:
                break;
            case DayManager.DayState.StartDay:
                break;

            case DayManager.DayState.StartWork:
                UpdateTutorialText("Walk over and press 'space' to review a candidate's resume. Be quick, as candidates quickly run out of patience.", TutorialTextType.PlaySlots, 0, globalDisappearDelay);
                break;
            case DayManager.DayState.EndWork:
                break;
            case DayManager.DayState.EndDay:
                break;
        }
    }

    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.myGameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                UpdateTutorialText("Press 'space' to interact with the slot machine.", TutorialTextType.PlayGame, playAppearDelay, playDisappearDelay);
            break;

            case GameFlowManager.GameAction.EnterMainMenu:
            case GameFlowManager.GameAction.PauseGame:
                background.SetActive(false);
            break;

            case GameFlowManager.GameAction.ResumeGame:
                background.SetActive(true);
            break;

            case GameFlowManager.GameAction.LoseGame:
            case GameFlowManager.GameAction.None:
            break;
        }
    }

    

    void UpdateTutorialText(string text, TutorialTextType myTextType, float appearDelayInSeconds, float dissapearDelayInSeconds)
        => StartCoroutine(UpdateTutorialText_CO(text, myTextType, appearDelayInSeconds, dissapearDelayInSeconds));

    IEnumerator UpdateTutorialText_CO(string text, TutorialTextType myTextType, float appearDelayInSeconds, float disappearDelayInSeconds)
    {
        if (displayedTips.Contains(myTextType))
            yield break;

        myCurrentTutorialText = myTextType;
        displayedTips.Add(myTextType);

        yield return new WaitForSeconds(appearDelayInSeconds);

        if (myCurrentTutorialText != myTextType && myCurrentTutorialText != TutorialTextType.ComputerEnter && myCurrentTutorialText != TutorialTextType.None)
            yield break;

        background.SetActive(true);
        tutorialText_TMP.text = text;

        yield return new WaitForSeconds(disappearDelayInSeconds);

        ClearText(myTextType);
    }

    void ClearText(TutorialTextType myTextToclear)
    {
        if (myCurrentTutorialText == myTextToclear)
        {
            background.SetActive(false);
            tutorialText_TMP.text = "";
            myCurrentTutorialText = TutorialTextType.None;
        }
    }
}

class TutorialText
{
    // Maybe I could define a tutorial text class, with a given set of predicates as its appear/disappear requirements

    // Or maybe it could be given enums, be set in the inspector, and respond appropriately to events
        // It would need to know the event to subscribe to, as well as the exact parameter to respond to, which would be a potentially large amount of fields

    // Either way this approach would take more time, but is worth looking into for the future
    
}
