using UnityEngine;

public class StationData : ScriptableObject
{
    [SerializeField] PlayerFocus.Station stationType;
    [SerializeField] LayerMask hitLayerMasks;
    [SerializeField] VirtualScreen virtualScreen;
}
