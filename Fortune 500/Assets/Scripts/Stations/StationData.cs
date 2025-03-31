using UnityEngine;

public class StationData : ScriptableObject
{
    [field: SerializeField] public PlayerFocus.Station StationType { get; private set; }
    [field: SerializeField] public LayerMask HitLayerMasks { get; private set; }
    [field: SerializeField] public VirtualScreen VirtualScreen { get; private set; }
}
