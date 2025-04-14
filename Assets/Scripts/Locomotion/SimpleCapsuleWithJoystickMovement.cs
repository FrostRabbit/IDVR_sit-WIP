using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Oculus.Interaction;
using Unity.VisualScripting;

public class SimpleCapsuleWithJoystickMovement : MonoBehaviour
{
    public bool EnableLinearMovement = true;
    public bool EnableRotation = true;
    public bool HMDRotatesPlayer = true;
    public float RotationAngle = 45.0f;
    public float Speed = 0.0f;
    public OVRCameraRig CameraRig;

    private bool ReadyToSnapTurn;
    private Rigidbody _rigidbody;

    public event Action CameraUpdated;
    public event Action PreCharacterMove;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
    }

    void Start()
    {
    }

    private void FixedUpdate()
    {
        if (CameraUpdated != null) CameraUpdated();
        if (PreCharacterMove != null) PreCharacterMove();

        if (HMDRotatesPlayer) RotatePlayerToHMD();
        if (EnableLinearMovement) JoystickMovement();
    }

    void RotatePlayerToHMD()
    {
        Transform root = CameraRig.trackingSpace;
        Transform centerEye = CameraRig.centerEyeAnchor;

        Vector3 prevPos = root.position;
        Quaternion prevRot = root.rotation;

        transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

        root.position = prevPos;
        root.rotation = prevRot;
    }

    /*************************************************************************************************
    TODO: Inplement StickMovement
    Variables:
    - cameraTransform (Transform): Transform of the camera or head representing the player's view
    - moveDir (Vector3): The direction the player should move in
    - primaryAxis (Vector2): Thumbstick input from the VR controller
    - forward (Vector3): The forward vector of the camera
    - right (Vector3): The right vector of the camera
    - Speed (float): The speed at which the player should move (modify in the Unity Editor, not here)
    - _rigidbody (Rigidbody): The Rigidbody component of the player
    *************************************************************************************************/
    void JoystickMovement()
    {
        // Initialize variables
        Vector3 forward = Vector3.zero;
        Vector3 right = Vector3.zero;

        // Assume CameraRig is the VR Rig, and centerEyeAnchor is the camera or head representing the player's view.
        Transform cameraTransform = CameraRig.centerEyeAnchor; 
        Vector3 moveDir = Vector3.zero;
        // Get thumbstick input from the VR controller, using Oculus' OVRInput
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // Hint:
        // 1. Get the forward and right vectors of the camera. get the forward vector by using cameraTransform.forward for example.
        // 2. Calculate the movement direction based on the thumbstick input and the "normalized" camera's forward and right vectors.
        // 3. Multiply the movement direction by the Speed variable to control the speed of the player.
        // 4. Make sure to multiply the movement direction by Time.fixedDeltaTime to make the movement frame rate independent.
        // 5. Use the Rigidbody component to move the player by setting its velocity to the movement direction. _rigidbody.MovePosition(Vector3 position) can be used to move the player.
        // Your code here:
        
        forward = cameraTransform.forward;
        right = cameraTransform.right;
        moveDir = (forward * primaryAxis.y + right * primaryAxis.x).normalized;

        // Move the player and fix the y velocity
        WIP wip = GetComponent<WIP>();
        Vector3 newVelocity = new Vector3(0, 0, 0);

        if(wip.isJump){
            //Debug.Log("Jumping");
            newVelocity = new Vector3(forward.x * wip.flySpeed, _rigidbody.velocity.y + wip.currentJumpforce , forward.z * wip.flySpeed);
            // if (wip.waveCount == 0 && flySpeed != 0.0f){
            //     newVelocity.y += 
            // }
        }else if(wip.moveSpeed > 0.0f){
            newVelocity = new Vector3(forward.x * wip.moveSpeed, _rigidbody.velocity.y, forward.z * wip.moveSpeed);
        }else{
            newVelocity = new Vector3(moveDir.x * Speed, _rigidbody.velocity.y, moveDir.z * Speed);
        }

        _rigidbody.velocity = newVelocity;
        
    }
}
