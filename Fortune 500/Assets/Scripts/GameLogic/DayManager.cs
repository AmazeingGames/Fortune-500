using UnityEngine;
using System.Collections.Generic;
using System;
using static DayStateChangeEventArgs;
using UnityEngine.Assertions;
using static UIButton;

public class DayManager : Singleton<DayManager> 
{
    [field: SerializeField] List<int> employeesPerDay = new();

    public static EventHandler<DayStateChangeEventArgs> DayStateChangeEventHandler;

    public DayState MyDayState { get; private set; }
    public int CurrentDay { get; private set; }
    public int RemainingEmployees { get; private set; }

    public enum DayState { None, StartDay, StartWork, EndWork, EndDay }

    public int EmployeesHiredToday { get; private set; } = 0;
    public int EmployeesRejectedToday { get; private set; } = 0;
    public int MistakesMadeToday { get; private set; } = 0;


    private void OnEnable()
    {
        CandidateHandler.ReviewedCandidateEventHandler += HandleHiredCandidate;
        SlotMachineButton.SlotsInteractEventHandler += HandlePullLever;
        GameFlowManager.PerformGameActionEventHandler += HandleGameAction;

        UIButton.UIInteractEventHandler += HandleUIInteract;
    }

    private void OnDisable()
    {
        CandidateHandler.ReviewedCandidateEventHandler -= HandleHiredCandidate;
        SlotMachineButton.SlotsInteractEventHandler -= HandlePullLever;
        GameFlowManager.PerformGameActionEventHandler -= HandleGameAction;
        UIButton.UIInteractEventHandler -= HandleUIInteract;
    }

    void HandleUIInteract(object sender, UIInteractEventArgs e)
    {
        if (e.buttonEvent != UIButton.EventType.DayAction)
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
    
    void HandleHiredCandidate(object sender, ReviewedCandidateEventArgs e)
    {
        RemainingEmployees--;

        Assert.IsFalse(RemainingEmployees < 0);

        if (!e.wasDecisionCorrect)
            MistakesMadeToday++;

        if (e.didHireCandidate)
            EmployeesHiredToday++;
        else
            EmployeesRejectedToday++;

        if (!e.DidLose && RemainingEmployees == 0)
            OnDayStateChange(DayState.EndWork);
    }

    void HandleGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                OnDayStateChange(DayState.StartDay);
            break;
        }
    }

    void OnDayStateChange(DayState myDayState)
    {
        switch (myDayState)
        {
            case DayState.StartDay:
                RemainingEmployees = CurrentDay < employeesPerDay.Count ? employeesPerDay[CurrentDay] : employeesPerDay[CurrentDay - 1];
                CurrentDay++;

                EmployeesHiredToday = 0;
                EmployeesRejectedToday = 0;
                MistakesMadeToday = 0;
            break;
        }

        DayStateChangeEventHandler?.Invoke(this, new(myDayState));
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

