using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static FocusStation;
//Potential Rename: "Focus Manager"
public class PlayerFocus : Singleton<PlayerFocus>
{
    public Station MyStation { get; private set; } = Station.Nothing;
    public Station MyPreviousStation { get; private set; } = Station.Nothing;

    public enum Station { Circuitry, Arcade, Nothing, Null, FrontView, BackView, RightView, LeftView }

    public FocusStation ClosestStation { get; private set; } = null;

    public static EventHandler<FocusAttemptedEventArgs> FocusAttemptedEventHandler;

    private void OnEnable()
    {
        FocusStation.InterfaceConnectedEventHandler += HandleConnectToStation;
        FocusStation.ProximityEnteredEventHandler += HandleProximityEnter;
    }

    private void OnDisable()
    {
        FocusStation.InterfaceConnectedEventHandler -= HandleConnectToStation;
        FocusStation.ProximityEnteredEventHandler -= HandleProximityEnter;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Focus"))
        {
            InteractionType myInteractionType = MyStation == Station.Nothing ? InteractionType.Connect : InteractionType.Disconnect;
            OnFocusAttempt(myInteractionType);
        }
    }

    void OnFocusAttempt(InteractionType interactionType)
        => FocusAttemptedEventHandler?.Invoke(this, new(interactionType));

    public void HandleConnectToStation(object sender, InterfaceConnectedEventArgs e)
    {
        MyPreviousStation = MyStation;
        MyStation = e.myInteractionType == InteractionType.Connect ? e.linkedStation : Station.Nothing;
    }

    public void HandleProximityEnter(object sender, ProximityEnteredEventArgs e)
        => ClosestStation = e.didEnter ? e.focusStation : null;

    public static bool IsFocusedOn(Station focusedOn) => (Instance == null || focusedOn == Instance.MyStation);
}

public class FocusAttemptedEventArgs : EventArgs
{
    public readonly InteractionType myInteractionType;
    public FocusAttemptedEventArgs(InteractionType myInteractionType)
        => this.myInteractionType = myInteractionType;
}
