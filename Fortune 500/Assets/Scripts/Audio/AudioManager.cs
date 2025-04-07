using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using static FocusStation;
using static Settings;
public class AudioManager : MonoBehaviour
{
    FMODEvents Events => FMODEvents.Instance;

    Dictionary<SoundData.ExclusiveType, List<SoundData>> ExclusiveTypeToSoundsData = new(); 

    // Instead of having a dictionary, we could just store the event instance directly inside of SoundData
    readonly Dictionary<SoundData, EventInstance> SoundDataToEventInstance = new();
    readonly List<EventInstance> pausedEventInstances = new();

    private void OnEnable()
    {
        FPSInput.TakeActionEventHandler += HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnect;
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
        GameFlowManager.PerformGameActionEventHandler += HandleGameAction;
        Settings.UpdateSettingsEventHandler += HandleUpdateSettings;
    }

    private void OnDisable()
    {
        FPSInput.TakeActionEventHandler -= HandlePlayerAction;
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnect;
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
        GameFlowManager.PerformGameActionEventHandler -= HandleGameAction;
        Settings.UpdateSettingsEventHandler -= HandleUpdateSettings;
    }

    private void Start()
    {
        ExclusiveTypeToSoundsData = new()
        {
            { SoundData.ExclusiveType.None, null },
            { SoundData.ExclusiveType.PhoneCall, new List<SoundData>(){ Events.LoseGame, Events.EndDayCall, Events.IntroCall } }
        };

        Assert.AreEqual(ExclusiveTypeToSoundsData.Count, Enum.GetNames(typeof(SoundData.ExclusiveType)).Length, "Not all exclusive types are covered within dictionary.");
    }

    void HandleUpdateSettings(object sender, UpdateSettingsEventArgs e)
    {
        foreach (SoundData soundData in ExclusiveTypeToSoundsData[SoundData.ExclusiveType.PhoneCall])
        {
            bool shouldMute = Settings.LinkedSettingToSetting[Settings.LinkedSetting.ShouldMuteCalls];
            soundData.SetMute(shouldMute);

            if (!SoundDataToEventInstance.TryGetValue(soundData, out EventInstance soundInstance))
                continue;

            if (shouldMute)
            {
                soundInstance.getVolume(out float volume);
                soundData.RememberVolume(volume);
                soundInstance.setVolume(0);
            }
            else
                soundInstance.setVolume(soundData.RememberedVolume);
        }
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
            case DayManager.DayState.StartDay:
                Events.AmbienceSound.start();
            break;

            case DayManager.DayState.EndWork:
                PlayOneShot(Events.EndDayCall, transform.position);
            break;
        }
    }

    void HandlePlayerAction(object sender, PlayerActionEventArgs e)
    {
        switch (e.myPlayerAction)
        {
            case PlayerActionEventArgs.PlayerActions.Step:
                Events.PlayerFootsteps.getPlaybackState(out PLAYBACK_STATE playbackState);

                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                    Events.PlayerFootsteps.start();
            break;

            case PlayerActionEventArgs.PlayerActions.Stop:
                Events.PlayerFootsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            break;
        }
    }

    public void HandleGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                PlayOneShot(Events.IntroCall, transform.position);
            break;

            case GameFlowManager.GameAction.LoseGame:
                PlayOneShot(Events.LoseGame, transform.position);
            break;

            case GameFlowManager.GameAction.PauseGame:
                // Debug.Log("paused game");
                Events.AmbienceSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                Events.PlayerFootsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                StopOneShot(Events.LoseGame,   playOnGameResume: true);
                StopOneShot(Events.EndDayCall, playOnGameResume: true);
                StopOneShot(Events.IntroCall,  playOnGameResume: true);
            break;

            case GameFlowManager.GameAction.ResumeGame:
                Events.AmbienceSound.start();
                foreach (var sound in pausedEventInstances)
                    sound.setPaused(false);
                pausedEventInstances.Clear();
            break;

            case GameFlowManager.GameAction.EnterMainMenu:
                pausedEventInstances.Clear();
            break;
        }
    }

    void StopOneShot(SoundData soundReference, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT, bool playOnGameResume = false)
    {
        if (!SoundDataToEventInstance.TryGetValue(soundReference, out var eventInstance))
        {
            Debug.Log($"Could not find sound {soundReference.MySoundType}");
            return;
        }

        // Debug.Log($"Stopped {soundReference.MySoundType}");

        if (playOnGameResume)
        {
            pausedEventInstances.Add(eventInstance);
            eventInstance.setPaused(true);
        }
        else
            eventInstance.stop(stopMode);
    }

    void PlayOneShot(SoundData soundDataToPlay, Vector3 soundOrigin, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
    {
        // Debug.Log($"Triggered Audio Clip: {soundDataToPlay.MySoundType}");
        EventInstance soundInstance = RuntimeManager.CreateInstance(soundDataToPlay.EventReference);
        
        if (SoundDataToEventInstance.TryGetValue(soundDataToPlay, out _))
            SoundDataToEventInstance[soundDataToPlay] = soundInstance;
        else
            SoundDataToEventInstance.Add(soundDataToPlay, soundInstance);

        if (ExclusiveTypeToSoundsData.TryGetValue(soundDataToPlay.MyExclusiveType, out List<SoundData> exclusiveSoundsData))
        {
            foreach (SoundData soundData in exclusiveSoundsData)
            {
                if (SoundDataToEventInstance.TryGetValue(soundData, out EventInstance overlappingSound))
                {
                    overlappingSound.stop(stopMode);
                    Debug.Log($"stopped {soundData.MySoundType}");
                }
            }
        }

        if (soundDataToPlay.IsMuted)
        {
            soundInstance.getVolume(out float volume);
            soundDataToPlay.RememberVolume(volume);
            soundInstance.setVolume(0);
        }

        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(soundOrigin));
        soundInstance.start();
        soundInstance.release();

    }
}