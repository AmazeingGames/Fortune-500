using UnityEngine;

public class VirtualScreenSceneData : ScriptableObject
{
    [SerializeField] PlayerFocus.Station stationType;
    [SerializeField] LayerMask hitLayerMasks;
}
