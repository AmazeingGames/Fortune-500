using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXY, MouseX, MouseY }

    public RotationAxes axes = RotationAxes.MouseXY;

    public float verticalSensitivity = 9f;
    public float horizontalSensitivity = 9f;

    public float minimumVerticalRotation = -45f;
    public float maximumVerticalRotation = 45f;

    float currentVerticalRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent<Rigidbody>(out var body))
            body.freezeRotation = true;
    }

    private void Update()
    {
        float delta = Input.GetAxis("Mouse X") * horizontalSensitivity;
        float horizontalRotation;

        switch (axes)
        {
            case RotationAxes.MouseXY:
                CalcVertRot();

                horizontalRotation = transform.localEulerAngles.y + delta;

                transform.localEulerAngles = new Vector3(currentVerticalRotation, horizontalRotation, 0);
            break;
            
            case RotationAxes.MouseX:
                transform.Rotate(0, delta, 0);
            break;

            case RotationAxes.MouseY:
                CalcVertRot();

                horizontalRotation = transform.localEulerAngles.y;

                transform.localEulerAngles = new Vector3(currentVerticalRotation, horizontalRotation, 0);
            break;
        }
    }

    void CalcVertRot()
    {
        currentVerticalRotation -= Input.GetAxis("Mouse Y") * verticalSensitivity;
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minimumVerticalRotation, maximumVerticalRotation);
    }

}
