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
    [SerializeField] private EventReference IntroCallEvent;
    [SerializeField] private EventReference EndDayCallEvent;
    [SerializeField] private EventReference LoseGameEvent;

    EventInstance AmbienceSound;

    void Start()
    {        
        masterBus = RuntimeManager.GetBus("bus:/");

        AmbienceSound = CreateInstance(FMODEvents.Instance.GameAmbience);
    }

    private void OnEnable()
    {
        FPSInput.TakeActionEventHandler += HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnect;
        GameManager.GameActionEventHandler += HandleGameAction;
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
    }

    private void OnDisable()
    {
        FPSInput.TakeActionEventHandler -= HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnect;
        GameManager.GameActionEventHandler -= HandleGameAction;
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
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

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayStateChangeEventArgs.DayState.StartWork:
                AmbienceSound.start();
            break;

            case DayStateChangeEventArgs.DayState.EndWork:
                PlayOneShot(EndDayCallEvent, transform.position);
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

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    public void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameManager.GameAction.PlayGame:
                PlayOneShot(IntroCallEvent, transform.position);
            break;

            case GameManager.GameAction.LoseGame:
                PlayOneShot(LoseGameEvent, transform.position);
            break;

            case GameManager.GameAction.PauseGame:
                AmbienceSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            break;
        }
    }

    public void PlayOneShot(EventReference sound, Vector3 origin)
    {
        Debug.Log($"Triggered Audio Clip: {sound}");

        if (Instance == null)
            return;

        RuntimeManager.PlayOneShot(sound, origin);
    }
}