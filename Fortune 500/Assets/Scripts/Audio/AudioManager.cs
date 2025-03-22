using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;
public class AudioManager : Singleton<AudioManager>
{
   Bus masterBus;

    readonly List<StudioEventEmitter> EventEmitters = new();

    void Start()
        => masterBus = RuntimeManager.GetBus("bus:/");

    private void OnEnable()
    {
        FPSInput.TakeActionEventHandler += HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnect;
    }

    private void OnDisable()
    {
        FPSInput.TakeActionEventHandler -= HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnect;
    }

    void HandleInterfaceConnect(object sender, InterfaceConnectedEventArgs e)
    {
        switch (e.myInteractionType)
        {
            case InteractionType.Connect:
            break;

            case InteractionType.Disconnect:
            break;

            case InteractionType.DoNothing:
            break;
        }
    }

    void HandlePlayerAction(object sender, PlayerActionEventArgs e)
    {
        switch (e.myPlayerAction)
        {
            case PlayerActionEventArgs.PlayerActions.Step:
                PlayOneShot(FMODEvents.Instance.Player3DFootsteps, e.origin);
            break;
        }
    }

    void PlayOneShot(EventReference sound, Vector3 origin)
    {
        Debug.Log($"Triggered Audio Clip: {sound}");

        if (Instance == null)
            return;

        RuntimeManager.PlayOneShot(sound, origin);
    }
}