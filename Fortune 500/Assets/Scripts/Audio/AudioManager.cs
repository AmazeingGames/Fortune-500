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
        switch (e.myLinkedSetting)
        {
            case LinkedSetting.None:
                break;
            case LinkedSetting.ToggleCalls:
                foreach (SoundData soundData in ExclusiveTypeToSoundsData[SoundData.ExclusiveType.PhoneCall])
                {
                    bool shouldMute = Settings.LinkedSettingToSetting[e.myLinkedSetting];
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
                break;
            case LinkedSetting.ToggleMusic:
                bool muteMusic = Settings.LinkedSettingToSetting[e.myLinkedSetting];
                if (muteMusic)
                { 
                    Events.OfficeMusicInstance.getVolume(out float officeVolume);
                    Events.OfficeMusic.RememberVolume(officeVolume);
                    Events.OfficeMusicInstance.setVolume(0);

                    Events.LobbyMusicInstance.getVolume(out float lobbyVolume);
                    Events.LobbyMusic.RememberVolume(lobbyVolume);
                    Events.LobbyMusicInstance.setVolume(0);
                }
                else
                {
                    Events.LobbyMusicInstance.setVolume(Events.LobbyMusic.RememberedVolume);
                    Events.OfficeMusicInstance.setVolume(Events.OfficeMusic.RememberedVolume);
                }
                break;
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
                Events.AmbienceInstance.start();
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
                Events.PlayerFootstepsInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                    Events.PlayerFootstepsInstance.start();
            break;

            case PlayerActionEventArgs.PlayerActions.Stop:
                Events.PlayerFootstepsInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            break;
        }
    }

    public void HandleGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.myGameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                PlayOneShot(Events.IntroCall, transform.position);
                
                Events.OfficeMusicInstance.start();
                Events.LobbyMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;

            case GameFlowManager.GameAction.LoseGame:
                PlayOneShot(Events.LoseGame, transform.position);
            break;

            case GameFlowManager.GameAction.PauseGame:
                // Debug.Log("paused game");
                Events.AmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                Events.PlayerFootstepsInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                Events.OfficeMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                Events.LobbyMusicInstance.start();

                StopOneShot(Events.LoseGame,   playOnGameResume: true);
                StopOneShot(Events.EndDayCall, playOnGameResume: true);
                StopOneShot(Events.IntroCall,  playOnGameResume: true);
            break;

            case GameFlowManager.GameAction.ResumeGame:
                Events.AmbienceInstance.start();
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