using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform cameraProxy;

    Quaternion cameraStartingRotation;

    Camera playerCamera;
    //float constantYPosition;

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

        StartCoroutine(SetCameraPosition(positionToSet, rotationToSet, e.myInteractionType));
    }

    IEnumerator SetCameraPosition(Vector3 positionToSet, Quaternion rotationToSet, FocusStation.InteractionType myInteractionType)
    {
        playerCamera.transform.SetPositionAndRotation(positionToSet, rotationToSet);

        if (myInteractionType == FocusStation.InteractionType.Connect)
            yield break;

        playerCamera.transform.position = cameraProxy.position;
    }
}
