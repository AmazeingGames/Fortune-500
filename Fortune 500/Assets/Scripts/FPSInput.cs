using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UnityEngine.CharacterController))]
[AddComponentMenu("Controls Script/FPS Input")]

public class FPSInput : MonoBehaviour
{
    [Header("Walk")]
    [SerializeField] float speed = 8f;
    [SerializeField] float gravity = -9f;

    [Header("Sound FX")]
    [SerializeField] float timeBetweenWalkSounds;
    [SerializeField] CharacterController characterController;

    float walkSoundTimer;
    float horizontalInput;
    float verticalInput;

    Vector3 movement;

    float deltaX;
    float deltaZ;

    public bool CanWalk { get; private set; } = true;

    // Update is called once per frame
    void Update()
    {
        MovePlayer();  
        CheckWalkSound();

        UpdateTimers();
        GetInput();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void UpdateTimers()
        => walkSoundTimer -= Time.deltaTime;

    void CheckWalkSound()
    {
        if (Mathf.Abs(horizontalInput) < .1f && Mathf.Abs(verticalInput) < .1f)
            return;

        if (Mathf.Abs(deltaX) < .1f && Mathf.Abs(deltaZ) < .1f)
            return;

        if (walkSoundTimer > 0)
            return;

        PlayWalkSound();
    }

    void PlayWalkSound()
    {
        walkSoundTimer = timeBetweenWalkSounds;
        AudioManager.TriggerAudioClip(AudioManager.OneShotSounds.Player3DFootsteps, transform);
    }

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
}
