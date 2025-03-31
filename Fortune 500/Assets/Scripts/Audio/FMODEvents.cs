using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : Singleton<FMODEvents>
{
    [field: Header("Ambiences")]
    [field: SerializeField] public EventReference GameAmbience { get; private set; }

    [field: Header("SFX")]

    [field: Header("UI")]
    [field: SerializeField] public EventReference UIHover { get; private set; }
    [field: SerializeField] public EventReference UISelect { get; private set; }

    [field: Header("Player")]
    [field: SerializeField] public EventReference Player3DFootsteps { get; private set; }

    [field: Header("Custscenes")]
    [field: SerializeField] public EventReference IntroCallEvent { get; private set; }
    [field: SerializeField] public EventReference EndDayCallEvent { get; private set ;}
    [field: SerializeField] public EventReference LoseGameEvent { get; private set ;}
}
