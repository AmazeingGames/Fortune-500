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

    void Start()
    {        
        masterBus = RuntimeManager.GetBus("bus:/");
    }

    private void OnEnable()
    {
        FPSInput.TakeActionEventHandler += HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnect;
        GameManager.GameActionEventHandler += HandleGameAction;
        
    }

    private void OnDisable()
    {
        FPSInput.TakeActionEventHandler -= HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnect;
        GameManager.GameActionEventHandler -= HandleGameAction;
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

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    public void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameManager.GameAction.None:
                break;
            case GameManager.GameAction.EnterMainMenu:
                break;
            case GameManager.GameAction.PlayGame:
                PlayOneShot(IntroCallEvent, transform.position);
                break;
            case GameManager.GameAction.StartDay:
                
                break;
            case GameManager.GameAction.PauseGame:
                break;
            case GameManager.GameAction.ResumeGame:
                break;
            case GameManager.GameAction.RestartDay:
                break;
            case GameManager.GameAction.LoadNextDay:
                break;
            case GameManager.GameAction.FinishDay:
                PlayOneShot(EndDayCallEvent, transform.position);
                break;
            case GameManager.GameAction.LoseGame:
                break;
            case GameManager.GameAction.StartWork:
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