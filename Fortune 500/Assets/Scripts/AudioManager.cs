using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.AnimationPlayableAsset;

public class AudioManager : Singleton<AudioManager>
{
    [field: Header("Game Ambiences")]
    [SerializeField] EventReference castleAmbience;
    [SerializeField] EventReference forestAmbience;
    [SerializeField] EventReference atticAmbience;

    [field: Header("Arcade Game SFX")]
    [SerializeField] EventReference playerJoust;
    [SerializeField] EventReference playerDamage;
    [SerializeField] EventReference arcadeFootsteps;
    [SerializeField] EventReference fireWall;
    [SerializeField] EventReference enemyDamage;

    [field: Header("Arcade Cabinet SFX")]
    [SerializeField] EventReference circuitCablePlug;
    [SerializeField] EventReference circuitCableUnplug;
    [SerializeField] EventReference arcadeOn;
    [SerializeField] EventReference arcadeOff;
    [SerializeField] EventReference circuitPanelOpen;
    [SerializeField] EventReference arcadeUIHover;
    [SerializeField] EventReference arcadeUISelect;
    [SerializeField] EventReference shakeArcade;

    [field: Header("UI")]
    [SerializeField] EventReference gameUIHover;
    [SerializeField] EventReference gameUISelect;

    [field: Header("3D Game")]
    [SerializeField] EventReference player3DFootsteps;
    [SerializeField] EventReference endingSounds;

    [field: Header("2D Game")]
    [SerializeField] EventReference consoleBlip;

    Bus masterBus;

    public enum OneShotSounds { Null, PlayerJoust, PlayerDamage, ArcadeFootsteps, CircuitCablePlug, CircuitCableUnplug, ArcadeOn, ArcadeOff, CircuitPanelOpen, Player3DFootsteps, ArcadeUIHover, ArcadeUISelect, UIHover, UISelect, ConsoleDialogue, Ending, ArcadeShake, EnemyTakeDamage }

    public enum LoopingAudio { Null, CastleAmbience, ForestAmbience, AtticAmbience, FireWall, PauseAmbience }

    public enum FootstepsParameter { Grass, Stone }

    public enum InstanceInteractMode { Start, StopAllowFadeout, StopImmediate }

    public enum LoopingAudioType { Null, Ambience }

    //Refers to the audio *file* correlated to the enum
    Dictionary<OneShotSounds, EventReference> OneShotToReference;

    //Refers to both the audio *file* location and the audio's priority value
    Dictionary<LoopingAudio, LoopingAudioReference> LoopingToReference;

    //Refers to the gameObject containing the 'EventInstance' script in scene
    readonly Dictionary<LoopingAudio, PriorityEventInstance> SoundTypeToEventInstance = new();


    readonly List<StudioEventEmitter> EventEmitters = new();

    void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");

        OneShotToReference = new()
        {
            { OneShotSounds.PlayerJoust, playerJoust},
            { OneShotSounds.PlayerDamage, playerDamage},
            { OneShotSounds.ArcadeFootsteps, arcadeFootsteps},
            { OneShotSounds.CircuitCablePlug, circuitCablePlug},
            { OneShotSounds.CircuitCableUnplug, circuitCableUnplug},
            { OneShotSounds.ArcadeOn, arcadeOn },
            { OneShotSounds.ArcadeOff, arcadeOff},
            { OneShotSounds.CircuitPanelOpen, circuitPanelOpen },
            { OneShotSounds.Player3DFootsteps, player3DFootsteps },
            { OneShotSounds.ArcadeUIHover, arcadeUIHover },
            { OneShotSounds.ArcadeUISelect, arcadeUISelect },
            { OneShotSounds.UIHover, gameUIHover },
            { OneShotSounds.UISelect, gameUISelect },
            { OneShotSounds.ConsoleDialogue, consoleBlip },
            { OneShotSounds.Ending, endingSounds },
            { OneShotSounds.ArcadeShake, shakeArcade },
            { OneShotSounds.EnemyTakeDamage, enemyDamage },
        };

        LoopingToReference = new()
        {
            { LoopingAudio.FireWall, new (LoopingAudio.FireWall, fireWall) },
            { LoopingAudio.CastleAmbience, new (LoopingAudio.CastleAmbience, castleAmbience, LoopingAudioType.Ambience) },
            { LoopingAudio.AtticAmbience,  new (LoopingAudio.AtticAmbience, atticAmbience, LoopingAudioType.Ambience) },
            { LoopingAudio.ForestAmbience, new (LoopingAudio.ForestAmbience, forestAmbience, LoopingAudioType.Ambience) },
        }; 
    }

    private void Update()
    {
        if (SettingsManager.Instance != null)
            masterBus.setVolume(SettingsManager.Instance.GameVolume);
    }

    public static void SetFootsteps(FootstepsParameter groundType)
        => RuntimeManager.StudioSystem.setParameterByName("TerrainType", (float)groundType);

    void OnDestroy()
        => CleanUp();

    // Make this have a default value of the caller's transform
    public static void TriggerAudioClip(OneShotSounds sound, Transform origin) => TriggerAudioClip(sound, origin.position);

    public static void TriggerAudioClip(OneShotSounds sound, Vector3 origin)
    {
        Debug.Log($"Triggered Audio Clip: {sound}");

        if (sound == OneShotSounds.Null)
            return;

        if (Instance == null)
            return;

        Instance.TriggerAudioClip(Instance.OneShotToReference[sound], origin);
    }

    void TriggerAudioClip(EventReference sound, Vector3 origin) => RuntimeManager.PlayOneShot(sound, origin);

    // Pass in the sound you want to play and the object to emit the sound
    // Supplies the audio event referencce and also keeps a list of emitter objects to be later disabled 
    public StudioEventEmitter InitializeEventEmitter(LoopingAudio key, GameObject emitterObject)
    {
        if (!LoopingToReference.TryGetValue(key, out var reference))
        {
            Debug.LogWarning($"No event reference found with the key \"{key}\"");
            return null;
        }

        if (!emitterObject.TryGetComponent<StudioEventEmitter>(out var emitter))
        {
            Debug.LogWarning("No event emitter component attached to object");
            return null;
        }

        Debug.Log($"Initialized emitter {key}");

        emitter.EventReference = reference.eventReference;
        EventEmitters.Add(emitter);
        return emitter;
    }

    // Given a looping sound, creates an instance of that sound that can be played on repeat
    PriorityEventInstance CreateEventInstance(LoopingAudio key)
    {
        LoopingToReference.TryGetValue(key, out LoopingAudioReference reference);

        PriorityEventInstance priorityEventInstance = new(reference);

        SoundTypeToEventInstance.Add(key, priorityEventInstance);

        return priorityEventInstance;
    }

    public static void StartEventTrack(LoopingAudio key) => EventInstanceInteract(key, InstanceInteractMode.Start);
    public static void StopEventTrack(LoopingAudio key) => EventInstanceInteract(key, InstanceInteractMode.StopAllowFadeout);

    // Used to start and stop looping audio
    // Creates audio of the given track, if none currently exists
    static void EventInstanceInteract(LoopingAudio key, InstanceInteractMode interactMode)
    {
        //This should instead instantiate the manager
        if (Instance == null)
        {
            Debug.LogWarning("AudioManager is null!");
            return;
        }

        // If no event instance exists, we create a new instance and recur
        if (!Instance.SoundTypeToEventInstance.TryGetValue(key, out PriorityEventInstance priorityEventInstance))
        {
            PriorityEventInstance newEventInstance = Instance.CreateEventInstance(key);

            Debug.Log($"Created new EventInstance (key) {key} | (instance) {newEventInstance.LoopingAudio}");

            EventInstanceInteract(key, interactMode);
            return;
        }

        EventInstanceStartOrStop(priorityEventInstance, interactMode);
    }

    // Responsbile for starting and stopping the given event instance
    // On starting an eventInstance, stops all other running event instances that match the given instance's priority
    static void EventInstanceStartOrStop(PriorityEventInstance priorityEventInstance, InstanceInteractMode interactMode)
    {
        var instance = priorityEventInstance.instance;
        switch (interactMode)
        {
            case InstanceInteractMode.Start:
                Debug.Log($"Started track : {priorityEventInstance.LoopingAudio}");
                instance.start();
                StopMatchingAudioType(priorityEventInstance);
                break;

            case InstanceInteractMode.StopAllowFadeout:
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;

            case InstanceInteractMode.StopImmediate:

                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;
        }
    }

    // Alternatively, I could set the volume to 0, this would allow me to set volume to custom values, in certain situations
    // Job is to loop through all created events that match a target's priority and stop all found tracks
    public static void StopMatchingAudioType(LoopingAudio currentAudio)
    {
        if (Instance == null)
            return;

        LoopingAudioType audioType = Instance.LoopingToReference[currentAudio].audioType;
        if (audioType == LoopingAudioType.Null)
            return;

        List<PriorityEventInstance> instances = Instance.SoundTypeToEventInstance.Values.ToList();

        Instance.StopMatching(instances, i => audioType == i.loopingAudioReference.audioType && currentAudio != i.LoopingAudio, $"of audioType {audioType}");
    }

    // Not sure why, but this function breaks the ambience manager when called
    static void StopAudioType(LoopingAudioType loopingAudioType)
    {
        if (Instance == null) 
            return;

        if (loopingAudioType == LoopingAudioType.Null)
            return;

        List<PriorityEventInstance> instances = Instance.SoundTypeToEventInstance.Values.ToList();

        Instance.StopMatching(instances, i => loopingAudioType == i.loopingAudioReference.audioType, $"of audioType {loopingAudioType}");
    }

    void StopMatching(List<PriorityEventInstance> priorityEventInstances, Predicate<PriorityEventInstance> stopCondition, string priorityString = "")
    {
        // Could use stringbuilder to improve performance
        string stoppedEventsString = string.Empty;

        foreach (var currentEventInstance in priorityEventInstances)
        {
            if (stopCondition(currentEventInstance))
            {
                EventInstanceStartOrStop(currentEventInstance, InstanceInteractMode.StopAllowFadeout);
                stoppedEventsString += $"{currentEventInstance.LoopingAudio}, ";
            }
        }

        if (stoppedEventsString == string.Empty)
            Debug.Log($"Stopped no events {priorityString}");
        else
            Debug.Log($"Stopped all matching events {priorityString} ({stoppedEventsString[..^2]})");
    }

    static void StopMatchingAudioType(PriorityEventInstance priorityEventInstance) => StopMatchingAudioType(priorityEventInstance.LoopingAudio);

    void CleanUp()
    {
        Debug.Log("CLEANUP");

        List<PriorityEventInstance> priorityEvents = SoundTypeToEventInstance.Values.ToList();
        foreach (PriorityEventInstance priorityEvent in priorityEvents)
        {
            priorityEvent.instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            priorityEvent.instance.release();
        }

        foreach (var eventEmitter in EventEmitters)
            eventEmitter.Stop();
    }

    // Stores a reference to a stored audio reference and a priority value
    // Purpose is to facilitate playing looping tracks while stopping tracks that have a conflicting priority value

    // Priority is a confusing name -- think of it more like a 'sound type'. No sound is more important than any other, except the one we want to currently play. All it does is stop sounds of a matching 'type'.
    readonly struct LoopingAudioReference
    {
        public readonly LoopingAudio eventKey;
        public readonly EventReference eventReference;
        public readonly LoopingAudioType audioType;

        // Sets 'no conflicts' as the default priority value
        public LoopingAudioReference(LoopingAudio eventKey, EventReference eventReference, LoopingAudioType audioType = LoopingAudioType.Null)
        {
            this.eventKey = eventKey;
            this.eventReference = eventReference;
            this.audioType = audioType;
        }
    }

    // Contains the sound, sound's priority, and sound reference
    // Used to start and stop sound instances
    readonly struct PriorityEventInstance
    {
        public readonly LoopingAudioReference loopingAudioReference;
        public readonly EventInstance instance;

        public EventReference EventReference => loopingAudioReference.eventReference;
        public LoopingAudio LoopingAudio => loopingAudioReference.eventKey;
        public PriorityEventInstance(LoopingAudioReference audioReference)
        {
            this.loopingAudioReference = audioReference;
            this.instance = RuntimeManager.CreateInstance(audioReference.eventReference);
        }
    }
}