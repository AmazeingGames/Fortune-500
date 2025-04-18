using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

//To fake functionality, only enable this component when it's being used
public class VirtualScreen : GraphicRaycaster
{
    [SerializeField] PlayerFocus.Station stationType;
    [SerializeField] LayerMask hitLayerMasks;

    [field: SerializeField] public PlayerFocus.Station StationType { get; private set; }
    [field: SerializeField] public LayerMask HitLayerMasks { get; private set; }

    //StationData stationData;
    public static EventHandler<FindStationDataEventArgs> FindStationDataEventHandler;

    // Camera responsible for rendering the virtual screen's rendertexture
    public Camera screenCamera;
    
    // Reference to the GraphicRaycaster of the canvas displayed on the virtual screen
    public GraphicRaycaster screenCaster; 

    public Transform lastMouseClick;

    protected override void Start()
    {
        base.Start();

        FindStationDataEventHandler?.Invoke(this, new(stationType, this));
    }

    // Called by Unity when a Raycaster should raycast because it extends BaseRaycaster.
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (eventCamera == null)
            return;

        Ray ray = eventCamera.ScreenPointToRay(eventData.position); // Mouse

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Figure out where the pointer would be in the second camera based on texture position or RenderTexture.
            Vector3 virtualPos = new(hit.textureCoord.x, hit.textureCoord.y);
            virtualPos.x *= screenCamera.targetTexture.width;
            virtualPos.y *= screenCamera.targetTexture.height;

            eventData.position = virtualPos;

            screenCaster.Raycast(eventData, resultAppendList);
        }
    }

}

public class FindStationDataEventArgs : EventArgs
{
    public readonly VirtualScreen virtualScreen;
    public readonly PlayerFocus.Station myStation;
    // public readonly StationData stationData;
    public FindStationDataEventArgs(PlayerFocus.Station myStation, VirtualScreen virtualScreen)
    {
        // this.stationData = stationData;
        this.virtualScreen = virtualScreen; 
        this.myStation = myStation;
    }
}




