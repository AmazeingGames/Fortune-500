using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;
public class AudioManager : MonoBehaviour
{
    Bus masterBus;
    
    readonly List<StudioEventEmitter> EventEmitters = new();

    EventInstance AmbienceSound;
    EventInstance playerFootsteps;

    FMODEvents Events => FMODEvents.Instance;

    void Start()
    {
        Debug.LogWarning("Audio Manager should be a single instance");
        masterBus = RuntimeManager.GetBus("bus:/");

        AmbienceSound = CreateInstance(Events.GameAmbience);
        playerFootsteps = CreateInstance(Events.Player3DFootsteps);
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
                PlayOneShot(Events.EndDayCallEvent, transform.position);
            break;
        }
    }


    void HandlePlayerAction(object sender, PlayerActionEventArgs e)
    {
        switch (e.myPlayerAction)
        {
            case PlayerActionEventArgs.PlayerActions.Step:
                playerFootsteps.getPlaybackState(out PLAYBACK_STATE playbackState);

                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                    playerFootsteps.start();
            break;

            case PlayerActionEventArgs.PlayerActions.Stop:
                playerFootsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
                PlayOneShot(Events.IntroCallEvent, transform.position);
            break;

            case GameManager.GameAction.LoseGame:
                PlayOneShot(Events.LoseGameEvent, transform.position);
            break;

            case GameManager.GameAction.PauseGame:
                AmbienceSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            break;
        }
    }

    void PlayOneShot(EventReference sound, Vector3 origin)
    {
        Debug.Log($"Triggered Audio Clip: {sound}");

        RuntimeManager.PlayOneShot(sound, origin);
    }
}