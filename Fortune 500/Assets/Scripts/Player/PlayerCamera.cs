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
    [SerializeField] float connectDuration;
    [SerializeField] float disconnectDuration;

    Camera playerCamera;
    //float constantYPosition;

    public static EventHandler<FinishedTweenEventArgs> FinishedTweenEventHandler;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
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
        switch (e.myInteractionType)
        {
            case FocusStation.InteractionType.Connect:
                cameraProxy.gameObject.SetActive(false);

                OnSetCameraPosition(e.cameraPosition.position, e.cameraPosition.rotation, e.myInteractionType);
                break;
            case FocusStation.InteractionType.Disconnect:
                cameraProxy.gameObject.SetActive(true);
                OnSetCameraPosition(cameraProxy.position, cameraProxy.rotation, e.myInteractionType);
                break;
        }
    }

    public bool setRotation;
    public bool setRotationDisconnect;

    Tween setRotationTween;
    Sequence sequence;
    void OnSetCameraPosition(Vector3 positionToSet, Quaternion rotationToSet, FocusStation.InteractionType myInteractionType)
    {
        if (myInteractionType == InteractionType.DoNothing)
            return;
        
        float duration = myInteractionType == InteractionType.Connect ? connectDuration : disconnectDuration;
        bool setRotation = myInteractionType == InteractionType.Connect ? this.setRotation : setRotationDisconnect;

        sequence?.Kill();
        setRotationTween?.Kill();
        
        sequence = DOTween.Sequence();
        sequence.Append(playerCamera.transform.DOMove(positionToSet, duration));

        if (setRotation)
            setRotationTween = playerCamera.transform.DORotateQuaternion(rotationToSet, duration);

        sequence.OnComplete(() => 
        { 
            sequence = null; 
            setRotationTween = null;

            FinishedTweenEventHandler?.Invoke(this, new(myInteractionType));
        });
    }
}

public class FinishedTweenEventArgs : EventArgs
{
    public readonly InteractionType myInteractionType;
    public FinishedTweenEventArgs(FocusStation.InteractionType myInteractionType)
    {
        this.myInteractionType = myInteractionType;
    }
}

