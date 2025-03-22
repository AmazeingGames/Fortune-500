using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;
using static AudioManager;
using static InterfaceConnectedEventArgs;
using UnityEngine.EventSystems;

public class FocusStation : MonoBehaviour
{
    public enum InteractionType { Connect, Disconnect, DoNothing };

    [SerializeField] Station myStation;
    [SerializeField] Transform stationCameraPosition;

    VirtualScreen linkedScreen;

    public static EventHandler<ProximityEnteredEventArgs> ProximityEnteredEventHandler;
    public static EventHandler<InterfaceConnectedEventArgs> InterfaceConnectedEventHandler;


    private void OnEnable()
    {
        PlayerFocus.FocusAttemptedEventHandler += HandleFocusAttempt;
        VirtualScreen.FindStation += HandleFindStation;
    }

    private void OnDisable()
    {
        PlayerFocus.FocusAttemptedEventHandler -= HandleFocusAttempt;
        VirtualScreen.FindStation -= HandleFindStation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null)
            return;

        if (!other.gameObject.CompareTag("Player"))
            return;

        OnProximityEnter(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == null)
            return;

        if (!other.gameObject.CompareTag("Player"))
            return;

        OnProximityEnter(false);
    }

    public void HandleFocusAttempt(object sender, FocusAttemptedEventArgs e)
    {
        if (linkedScreen == null)
            return;

        if (PlayerFocus.Instance.ClosestStation != this && PlayerFocus.Instance.MyStation != myStation)
        {
            linkedScreen.enabled = false;
            return;
        }

        linkedScreen.enabled = true;
        OnInterfaceConnect(e.myInteractionType);
    }


    void OnInterfaceConnect(InteractionType myInteractionType)
    {
        InterfaceConnectedEventHandler?.Invoke(this, new InterfaceConnectedEventArgs(myStation, myInteractionType, stationCameraPosition));
    }

    public void HandleArrowFocusAttempt(Station stationType)
    {
        InteractionType myInteractionType;

        // Neither connecting nor disconnecting
        if (stationType != myStation && PlayerFocus.Instance.MyStation != myStation)
        {
            if (linkedScreen != null)
                linkedScreen.enabled = false;

            myInteractionType = InteractionType.DoNothing;
        }
        myInteractionType = stationType == myStation ? InteractionType.Connect : InteractionType.Disconnect;

        // Either connecting or disconnecting
        if (linkedScreen != null)
            linkedScreen.enabled = true;

        OnInterfaceConnect(myInteractionType);

        Debug.Log($"InteractionType: {myInteractionType} | linkedStation : {myStation}");
    }
    void HandleFindStation(VirtualScreen sender, Station virtualScreenType)
    {
        if (virtualScreenType == myStation)
        {
            linkedScreen = sender;
            linkedScreen.enabled = false;

            Debug.Log($"Found screen! Linked Screen null : {linkedScreen == null}");
        }
    }

    void OnProximityEnter(bool playerEntering)
        => ProximityEnteredEventHandler?.Invoke(this, new(playerEntering, this));
}

public class InterfaceConnectedEventArgs : EventArgs
{
    public readonly Station linkedStation;
    public readonly FocusStation.InteractionType myInteractionType;
    public readonly Transform stationCamera;

    public InterfaceConnectedEventArgs(Station linkedStation, FocusStation.InteractionType myInteractionType, Transform stationCamera)
    {
        this.linkedStation = linkedStation;
        this.myInteractionType = myInteractionType;
        this.stationCamera = stationCamera;
    }
}

public class ProximityEnteredEventArgs : EventArgs
{
    public readonly FocusStation focusStation;
    public readonly bool didEnter;

    public ProximityEnteredEventArgs(bool didEnter, FocusStation focusStation)
    {
        this.focusStation = focusStation;
        this.didEnter = didEnter;
    }
}
