using DG.Tweening;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform cameraProxy;
    [SerializeField] MMTween tween;

    Quaternion cameraStartingRotation;

    Camera playerCamera;
    //float constantYPosition;

    public static EventHandler<SetCameraPositionEventArgs> SetCameraPositionEventHandler;

    void Start()
    {
        playerCamera = GetComponent<Camera>();

        //Note we actually don't need or care about these values on start; their only purpose on start is for debugging reasons
        cameraStartingRotation = playerCamera.transform.rotation;

        cameraProxy.position = playerCamera.transform.position;

    }

    private void OnEnable()
    {
        FocusStation.InterfaceConnectedEventHandler += HandleConnectToStation;
    }

    private void OnDisable()
    {
        FocusStation.InterfaceConnectedEventHandler -= HandleConnectToStation;
    }

    //Moves the camera when focusing/unfocusing
    void HandleConnectToStation(object sender, InterfaceConnectedEventArgs e)
    {
        Vector3 positionToSet;
        Quaternion rotationToSet;

        switch (e.myInteractionType)
        {
            case FocusStation.InteractionType.Connect:
                cameraStartingRotation = playerCamera.transform.rotation;

                positionToSet = e.cameraPosition.position;
                rotationToSet = e.cameraPosition.rotation;
                break;
            case FocusStation.InteractionType.Disconnect:
                positionToSet = cameraProxy.position;
                rotationToSet = cameraStartingRotation;
                break;
            default:
                return;
        }

        OnSetCameraPosition(positionToSet, rotationToSet, e.myInteractionType);
    }

    void OnSetCameraPosition(Vector3 positionToSet, Quaternion rotationToSet, FocusStation.InteractionType myInteractionType)
    {
        if (myInteractionType == FocusStation.InteractionType.Connect)
        {
            playerCamera.transform.SetPositionAndRotation(positionToSet, rotationToSet);
            SetCameraPositionEventHandler?.Invoke(this, new(positionToSet, rotationToSet, myInteractionType));
        }
        else if (myInteractionType == InteractionType.Disconnect)
        {
            playerCamera.transform.SetPositionAndRotation(cameraProxy.position, rotationToSet);
            SetCameraPositionEventHandler?.Invoke(this, new(cameraProxy.position, rotationToSet, myInteractionType));
        }
    }
}

public class SetCameraPositionEventArgs : EventArgs
{
    public readonly Vector3 positionToSet;
    public readonly Quaternion rotationToSet;
    public readonly FocusStation.InteractionType myInteractionType;

    public SetCameraPositionEventArgs(Vector3 positionToSet, Quaternion rotationToSet, FocusStation.InteractionType myInteractionType)
    {
        this.positionToSet = positionToSet;
        this.rotationToSet = rotationToSet;
        this.myInteractionType = myInteractionType;
    }
}

