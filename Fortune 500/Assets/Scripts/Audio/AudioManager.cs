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

    FMODEvents Events => FMODEvents.Instance;

    void Start()
    {        
        masterBus = RuntimeManager.GetBus("bus:/");
    }

    private void OnEnable()
    {
        FPSInput.TakeActionEventHandler += HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnect;
        GameFlowManager.PerformActionEventHandler += HandleGameAction;
        
    }

    private void OnDisable()
    {
        FPSInput.TakeActionEventHandler -= HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnect;
        GameFlowManager.PerformActionEventHandler -= HandleGameAction;
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
        EventReference soundToPlay = e.gameAction switch
        {
            GameFlowManager.GameAction.PlayGame => Events.IntroCall,

            GameFlowManager.GameAction.FinishDay => Events.EndDayCall,

            GameFlowManager.GameAction.LoseGame => Events.LoseGame,
            _ => new()
        };

        PlayOneShot(soundToPlay, transform.position);
    }

    void PlayOneShot(EventReference sound, Vector3 origin)
    {
        Debug.Log($"Triggered Audio Clip: {sound}");
        RuntimeManager.PlayOneShot(sound, origin);
    }
}