using UnityEngine;
using System.Collections.Generic;
using System;
using static DayStateChangeEventArgs;
using UnityEngine.Assertions;
using static UIButton;

public class DayManager : MonoBehaviour
{
    [field: SerializeField] List<int> employeesPerDay = new();

    public static EventHandler<DayStateChangeEventArgs> DayStateChangeEventHandler;

    public DayState MyDayState { get; private set; }
    public DayState MyPreviousDayState { get; private set; }
    public static int CurrentDay { get; private set; }
    public static int RemainingEmployees { get; private set; }

    public enum DayState { None, StartDay, StartWork, EndWork, EndDay }

    public static int EmployeesHiredToday { get; private set; } = 0;
    public static int EmployeesRejectedToday { get; private set; } = 0;
    public static int MistakesMadeToday { get; private set; } = 0;


    private void OnEnable()
    {
        CandidateHandler.ReviewedCandidateEventHandler += HandleReviewedCandidate;
        SlotMachineButton.SlotsInteractEventHandler += HandlePullLever;
        GameFlowManager.PerformGameActionEventHandler += HandleGameAction;
        UIButton.UIInteractEventHandler += HandleUIInteract;
    }

    private void OnDisable()
    {
        CandidateHandler.ReviewedCandidateEventHandler -= HandleReviewedCandidate;
        SlotMachineButton.SlotsInteractEventHandler -= HandlePullLever;
        GameFlowManager.PerformGameActionEventHandler -= HandleGameAction;
        UIButton.UIInteractEventHandler -= HandleUIInteract;
    }

    private void Update()
    {
        currentDay = CurrentDay;

    }

    void HandleUIInteract(object sender, UIInteractEventArgs e)
    {
        bool isDayAction = e.myButtonType == UIButton.ButtonType.DayAction;
        bool isValidDayState = e.myNewDayState != DayState.None;
        bool didClickButton = e.myInteractionType == UIInteractionTypes.Click;

        if (!isDayAction || !isValidDayState || !didClickButton)
            return;

        OnDayStateChange(e.myNewDayState);
    }

    void HandlePullLever(object sender, SlotsInteractEventArgs e)
    {
        switch (e.myInteractionType)
        {
            case SlotsInteractEventArgs.InteractionType.PullLever:
                OnDayStateChange(DayState.StartWork);
            break;
        }
    }
    
    void HandleReviewedCandidate(object sender, ReviewedCandidateEventArgs e)
    {
        RemainingEmployees--;

        Assert.IsFalse(RemainingEmployees < 0);

        if (!e.wasDecisionCorrect)
            MistakesMadeToday++;

        if (e.didHireCandidate)
            EmployeesHiredToday++;
        else
            EmployeesRejectedToday++;


        bool didLose = ScoreKeeper.StrikesLeft == 0;
        bool reviewedAllCandidates = RemainingEmployees == 0;

        if (!didLose && reviewedAllCandidates)
            OnDayStateChange(DayState.EndWork);
    }

    void HandleGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                CurrentDay = 0;
                EmployeesHiredToday = 0;
                EmployeesRejectedToday = 0;
                MistakesMadeToday = 0;
                OnDayStateChange(DayState.StartDay);
            break;
        }
    }

    int currentDay;
    void OnDayStateChange(DayState myNewDayState)
    {
        MyPreviousDayState = MyDayState;
        MyDayState = myNewDayState; 

        switch (myNewDayState)
        {
            case DayState.StartDay:

                RemainingEmployees = CurrentDay < employeesPerDay.Count ? employeesPerDay[CurrentDay] : employeesPerDay[CurrentDay - 1];
                CurrentDay++;

                EmployeesHiredToday = 0;
                EmployeesRejectedToday = 0;
                MistakesMadeToday = 0;
            break;
        }

        DayStateChangeEventHandler?.Invoke(this, new(myNewDayState));

        Debug.Log($"Changed day state to {myNewDayState}, from {MyPreviousDayState}");
    }
}

public class DayStateChangeEventArgs : EventArgs
{
    public readonly DayManager.DayState myDayState;

    public DayStateChangeEventArgs(DayManager.DayState myDayState)
    {
        this.myDayState = myDayState;
    }
}

