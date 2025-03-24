using UnityEngine;

public class VirtualScreenData : ScriptableObject
{
    [SerializeField] PlayerFocus.Station stationType;
    [SerializeField] LayerMask hitLayerMasks;
}
