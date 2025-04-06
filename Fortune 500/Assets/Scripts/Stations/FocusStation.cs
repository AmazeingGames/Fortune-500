using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;
using static AudioManager;
using static InterfaceConnectedEventArgs;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class FocusStation : MonoBehaviour
{
    public enum InteractionType { Connect, Disconnect, DoNothing };

    [SerializeField] Station myStation;
    [SerializeField] Transform cameraPosition;
    [SerializeField] ViewCheck viewCheck;
    [SerializeField] VirtualScreen virtualDisplay;

    public static EventHandler<ProximityEnteredEventArgs> ProximityEnteredEventHandler;
    public static EventHandler<InterfaceConnectedEventArgs> InterfaceConnectedEventHandler;

    private void OnEnable()
    {
        PlayerFocus.FocusAttemptedEventHandler += HandleFocusAttempt;
        VirtualScreen.FindStationDataEventHandler += HandleFindStation;
    }

    private void OnDisable()
    {
        PlayerFocus.FocusAttemptedEventHandler -= HandleFocusAttempt;
        VirtualScreen.FindStationDataEventHandler -= HandleFindStation;
    }

    private void Start()
    {
        cameraPosition.gameObject.SetActive(false);
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
        if (virtualDisplay == null)
            return;

        bool isClosestStation = PlayerFocus.Instance.ClosestStation == this;
        bool isPlayerConnected = PlayerFocus.Instance.MyConnectedStation == myStation;
        bool isBeingLookedAt = viewCheck.IsPlayerFacing && viewCheck.IsCameraFacing;

        if (e.myInteractionType == InteractionType.Disconnect && isPlayerConnected)
            OnInterfaceConnected(e.myInteractionType);
        else if (e.myInteractionType == InteractionType.Connect && isClosestStation && isBeingLookedAt)
            OnInterfaceConnected(e.myInteractionType);
        else
            virtualDisplay.enabled = false;
    }

    void OnInterfaceConnected(InteractionType myInteractionType)
    {
        Assert.IsTrue(myInteractionType != InteractionType.DoNothing);

        virtualDisplay.enabled = myInteractionType == InteractionType.Connect;
        InterfaceConnectedEventHandler?.Invoke(this, new InterfaceConnectedEventArgs(myStation, myInteractionType, cameraPosition.transform));
    }

    void HandleFindStation(object sender, FindStationDataEventArgs e)
    {
        if (e.myStation == myStation)
        {
            virtualDisplay = e.virtualScreen;
            virtualDisplay.enabled = false;

            // Debug.Log($"Found screen! Linked Screen null : {virtualDisplay == null}");
        }
    }

    void OnProximityEnter(bool playerEntering)
    {
        ProximityEnteredEventHandler?.Invoke(this, new(playerEntering, this));
    }
}

public class InterfaceConnectedEventArgs : EventArgs
{
    public readonly Station myStation;
    public readonly FocusStation.InteractionType myInteractionType;
    public readonly Transform cameraPosition;

    public InterfaceConnectedEventArgs(Station myStation, FocusStation.InteractionType myInteractionType, Transform cameraPosition)
    {
        this.myStation = myStation;
        this.myInteractionType = myInteractionType;
        this.cameraPosition = cameraPosition;
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
