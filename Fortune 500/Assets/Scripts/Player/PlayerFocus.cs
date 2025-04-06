using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static FocusStation;

public class PlayerFocus : Singleton<PlayerFocus>
{
    public Station MyConnectedStation { get; private set; } = Station.Nothing;
    public Station MyPreviousStation { get; private set; } = Station.Nothing;

    public enum Station { Nothing, Computer, Desk, Slots }

    public FocusStation ClosestStation { get; private set; } = null;

    public static EventHandler<FocusAttemptedEventArgs> FocusAttemptedEventHandler;

    private void OnEnable()
    {
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnection;
        FocusStation.ProximityEnteredEventHandler += HandleProximityEnter;
    }

    private void OnDisable()
    {
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnection;
        FocusStation.ProximityEnteredEventHandler -= HandleProximityEnter;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Focus"))
            OnFocusAttempt();
    }

    void OnFocusAttempt()
    {
        InteractionType myInteractionType = MyConnectedStation == Station.Nothing ? InteractionType.Connect : InteractionType.Disconnect;
        FocusAttemptedEventHandler?.Invoke(this, new(myInteractionType));

        // Debug.Log($"Focus attempted: {myInteractionType} to {ClosestStation}");
    }

    public void HandleInterfaceConnection(object sender, InterfaceConnectedEventArgs e)
    {
        MyPreviousStation = MyConnectedStation;
        MyConnectedStation = e.myInteractionType == InteractionType.Connect ? e.myStation : Station.Nothing;
    }

    public void HandleProximityEnter(object sender, ProximityEnteredEventArgs e)
    {
        ClosestStation = e.didEnter ? e.focusStation : null;
        // Debug.Log($"Player | HandledFocusStationProximityEnter: {(e.didEnter ? "Entered" : "Exit")} {e.focusStation} proximity.");
    }

    public static bool IsFocusedOn(Station focusedOn) => (Instance == null || focusedOn == Instance.MyConnectedStation);
}

public class FocusAttemptedEventArgs : EventArgs
{
    public readonly InteractionType myInteractionType;
    public FocusAttemptedEventArgs(InteractionType myInteractionType)
        => this.myInteractionType = myInteractionType;
}
