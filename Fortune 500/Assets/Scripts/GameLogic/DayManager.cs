using UnityEngine;
using System.Collections.Generic;
using System;
using static DayStateChangeEventArgs;
using UnityEngine.Assertions;

public class DayManager : MonoBehaviour
{
    [field: SerializeField] List<int> employeesPerDay = new();

    public static EventHandler<DayStateChangeEventArgs> DayStateChangeEventHandler;

    public DayState MyDayState { get; private set; }
    public int CurrentDay { get; private set; }
    public int RemainingEmployees { get; private set; }

    private void OnEnable()
    {
        CandidateHandler.HireCandidateEventHandler += HandleHiredCandidate;
        SlotMachineButton.SlotsInteractEventHandler += HandlePullLever;
        GameFlowManager.PerformActionEventHandler += HandleGameAction;
    }

    private void OnDisable()
    {
        CandidateHandler.HireCandidateEventHandler -= HandleHiredCandidate;
        SlotMachineButton.SlotsInteractEventHandler -= HandlePullLever;
        GameFlowManager.PerformActionEventHandler -= HandleGameAction;
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

    void HandleHiredCandidate(object sender, HiredCandidateEventArgs e)
    {
        RemainingEmployees--;

        Assert.IsFalse(RemainingEmployees < 0);

        if (RemainingEmployees == 0)
            OnDayStateChange(DayState.EndWork);
    }

    void HandleGameAction(object sender, GameActionEventArgs e)
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
            break;
        }

        DayStateChangeEventHandler?.Invoke(this, new(myDayState));
    }
}


public class GunDataEventArgs : ScriptableObject
{
    public readonly int bullets;
    public readonly string name;

    public GunDataEventArgs(int bullets, string name)
    {
        this.bullets = bullets;
        this.name = name;
    }
}

public class DayStateChangeEventArgs : EventArgs
{
    public enum DayState { StartDay, StartWork, EndWork, EndDay }
    public readonly DayState myDayState;

    public DayStateChangeEventArgs(DayState myDayState)
    {
        this.myDayState = myDayState;
    }
}

