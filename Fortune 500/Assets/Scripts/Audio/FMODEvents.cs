using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : Singleton<FMODEvents>
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference GameAmbience { get; private set; }

    [field: Header("UI Actions")]
    [field: SerializeField] public EventReference UIHover { get; private set; }
    [field: SerializeField] public EventReference UISelect { get; private set; }

    [field: Header("Player")]
    [field: SerializeField] public EventReference Player3DFootsteps { get; private set; }

    [field: Header("Custscenes")]
    [field: SerializeField] public EventReference LoseGame { get; private set; }
    [field: SerializeField] public EventReference IntroCall { get; private set; }
    [field: SerializeField] public EventReference EndDayCall { get; private set; }

    [field: Header("Resume Actions")]
    [field: SerializeField] public EventReference GoodHire { get; private set; }
    [field: SerializeField] public EventReference BadHire { get; private set; }

}
