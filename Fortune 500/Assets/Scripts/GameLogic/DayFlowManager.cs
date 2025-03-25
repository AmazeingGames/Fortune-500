using System;
using UnityEngine;
using static GameFlowManager;
using System.Collections.Generic;
using System.Linq;

public class DayFlowManager : MonoBehaviour
{
    [SerializeField] TextAsset candidatesPerDayFile;

    readonly Dictionary<int, int> dayToDailyCandidates = new();

    public enum DayAction { None, BeforeWork, DuringWork, AfterWork }
    /// <summary> Checks if we've interviewed all the candidates for the day. </summary>

    public DayAction MyLastDayAction { get; private set; } = DayAction.None;
    public DayAction MyPreviousDayAction { get; private set;  } = DayAction.None;
    public static EventHandler<DayStateChangedEventArgs> ChangedDayState;

    public int CurrentDay { get; private set; }
    public int RemainingCandidates { get; private set; }
    public int DailyCandidates { get; private set; }

    private void OnEnable()
    {
        CandidateHandler.HiredCandidateEventHandler += HandleHireCandidate;
        GameFlowManager.GameActionEventHandler += HandleGameAction;
    }

    private void OnDisable()
    {
        CandidateHandler.HiredCandidateEventHandler -= HandleHireCandidate;
        GameFlowManager.GameActionEventHandler -= HandleGameAction;
    }
    private void Start()
    {
        var list = (List<int>)GetListFromTextAsset(candidatesPerDayFile).Cast<int>();
        for (int i = 0; i < list.Count; i++)
            dayToDailyCandidates.Add(i, list[i]);
    }

    List<string> GetListFromTextAsset(TextAsset textAsset)
        => textAsset ? new(textAsset.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)) : null;

    void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameAction.StartDay:
                RemainingCandidates = CurrentDay >= dayToDailyCandidates.Count ? dayToDailyCandidates[dayToDailyCandidates.Count - 1] : dayToDailyCandidates[CurrentDay];
            break;
        }
    }

    void HandleHireCandidate(object sender, HiredCandidateEventArgs e)
    {
        RemainingCandidates--;

        if (RemainingCandidates <= 0)
            throw new Exception("Remaining candidates should not ever be able to go below 0.");

        if (RemainingCandidates == 0)
            OnChangeDayState(DayAction.AfterWork);
    }

    void OnChangeDayState(DayAction myNewDayState)
    {
        MyLastDayAction = myNewDayState;
        ChangedDayState?.Invoke(this, new(myNewDayState, MyPreviousDayAction));
    }
}

public class DayStateChangedEventArgs : EventArgs
{
    public readonly DayFlowManager.DayAction myDayState;
    public readonly DayFlowManager.DayAction previousDayState;

    public DayStateChangedEventArgs(DayFlowManager.DayAction myNewDayState, DayFlowManager.DayAction previousDayState)
    {
        this.myDayState = myNewDayState;
        this.previousDayState = previousDayState;
    }
}

