using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static PlayerActionEventArgs;
using FMOD.Studio;

[RequireComponent(typeof(UnityEngine.CharacterController))]
[AddComponentMenu("Controls Script/FPS Input")]

public class FPSInput : MonoBehaviour
{
    [Header("Walk")]
    [SerializeField] float speed = 8f;
    [SerializeField] float gravity = -9f;

    [Header("Sound FX")]
    [SerializeField] float timeBetweenWalkSounds;

    [Header("Components")]
    [SerializeField] CharacterController characterController;

    bool lockMovement = false;
    float walkSoundTimer;
    float horizontalInput;
    float verticalInput;

    Vector3 movement;

    float deltaX;
    float deltaZ;

    public bool CanWalk { get; private set; } = true;

    public static EventHandler<PlayerActionEventArgs> TakeActionEventHandler;

    private void OnEnable()
    {
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnection;
    }

    private void OnDisable()
    {
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnection;
    }

    private void Awake()
    {
    }

    void HandleInterfaceConnection(object sender, InterfaceConnectedEventArgs e)
    {
        lockMovement = e.myInteractionType switch
        {
            FocusStation.InteractionType.Connect => true,
            FocusStation.InteractionType.Disconnect => false,
            _ => lockMovement,
        };
    }


    void Update()
    {
        UpdateTimers();
        
        if (!lockMovement)
            MovePlayer();
        GetInput();
        UpdateWalk();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void UpdateTimers()
        => walkSoundTimer -= Time.deltaTime;


    void MovePlayer()
    {
        deltaX = horizontalInput * speed;
        deltaZ = verticalInput * speed;

        movement = new(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        movement.y = gravity;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);

        characterController.Move(movement);
    }

    void OnTakeAction(PlayerActions myPlayerAction)
    {
        TakeActionEventHandler?.Invoke(this, new(myPlayerAction, transform.position));
    }

    private void UpdateWalk()
    {
        if (Mathf.Abs(horizontalInput) < .1f && Mathf.Abs(verticalInput) < .1f)
            OnTakeAction(PlayerActions.Stop);
        else if (Mathf.Abs(deltaX) < .1f && Mathf.Abs(deltaZ) < .1f)
            OnTakeAction(PlayerActions.Stop);
        else if (lockMovement)
            OnTakeAction(PlayerActions.Stop);

        OnTakeAction(PlayerActions.Step);
    }
}

public class PlayerActionEventArgs : EventArgs
{
    public enum PlayerActions { Step, Stop }
    public PlayerActions myPlayerAction;
    public Vector3 origin;

    public PlayerActionEventArgs(PlayerActions myPlayerAction, Vector3 origin)
    {
        this.myPlayerAction = myPlayerAction;
        this.origin = origin;
    }
}
