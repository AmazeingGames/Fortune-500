using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public class FMODEvents : Singleton<FMODEvents>
{
    [field: Header("Ambience")]
    [field: SerializeField] public SoundData GameAmbience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public SoundData LobbyMusic { get; private set; }
    [field: SerializeField] public SoundData OfficeMusic { get; private set; }

    [field: Header("UI Actions")]
    [field: SerializeField] public SoundData UIHover { get; private set; }
    [field: SerializeField] public SoundData UISelect { get; private set; }

    [field: Header("Player")]
    [field: SerializeField] public SoundData Player3DFootsteps { get; private set; }

    [field: Header("Custscenes")]
    [field: SerializeField] public SoundData LoseGame { get; private set; }
    [field: SerializeField] public SoundData IntroCall { get; private set; }
    [field: SerializeField] public SoundData EndDayCall { get; private set; }

    [field: Header("Resume Actions")]
    [field: SerializeField] public SoundData GoodHire { get; private set; }
    [field: SerializeField] public SoundData BadHire { get; private set; }

    public Bus MasterBus {  get; private set; }

    public EventInstance AmbienceInstance { get; private set; }
    public EventInstance PlayerFootstepsInstance { get; private set; }
    public EventInstance OfficeMusicInstance { get; private set; }
    public EventInstance LobbyMusicInstance { get; private set; }

    void Start()
    {
        
        MasterBus = RuntimeManager.GetBus("bus:/");

        AmbienceInstance = CreateInstance(GameAmbience.EventReference);
        PlayerFootstepsInstance = CreateInstance(Player3DFootsteps.EventReference);
        LobbyMusicInstance = CreateInstance(LobbyMusic.EventReference);
        OfficeMusicInstance = CreateInstance(OfficeMusic.EventReference);
    }

    EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }
}

[Serializable]
public class SoundData
{
    public enum ExclusiveType { None, PhoneCall }
    public enum SoundType { None, GameAmbience, UIHover, UISelect, Player3DFootsteps, LoseGame, IntroCall, EndDayCall, GoodHire, BadHire, LobbyMusic, OfficeMusic }

    public bool IsMuted { get; private set; }
    public float RememberedVolume { get; private set; }

    [field: SerializeField] public EventReference EventReference { get; private set; }
    [field: SerializeField] public ExclusiveType MyExclusiveType { get; private set; }
    [field: SerializeField] public SoundType MySoundType { get; private set; }

    public void SetMute(bool shouldMute)
        => IsMuted = shouldMute;

    public void RememberVolume(float volumeToRemember)
        => RememberedVolume = volumeToRemember;
}

