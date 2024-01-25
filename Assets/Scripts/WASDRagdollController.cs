using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDRagdollController : MonoBehaviour
{
    private static readonly int SpeedAnimator = Animator.StringToHash("Speed");
    
    [SerializeField]
    private Animator puppeteerAnimator;

    [SerializeField]
    private Rigidbody root;

    [SerializeField]
    private Transform cameraTransform;
    [SerializeField, Min(0)]
    private float speed;

    private Vector2 _inputDirections;
    private bool _hasInput;

    private bool _ragdollActive;

    [SerializeField]
    private LayerMask groundMask;

    private float _groundHeightOffset;
    private float _heightOffGround;

    //============================================================================================================//

    private void OnEnable()
    {
        PuppetRagdoll.OnRagdollActive += OnRagdollActive;
    }



    // Start is called before the first frame update
    private void Start()
    {
        CheckGroundHeight(out _groundHeightOffset);
        _groundHeightOffset *= 0.95f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_ragdollActive)
            return;
        
        UpdateInputDirection(ref _inputDirections);
        CheckGroundHeight(out _heightOffGround);

        _hasInput = _inputDirections != Vector2.zero;

        if (_hasInput == false)
        {
            puppeteerAnimator.SetFloat(SpeedAnimator, 0f);
            return;
        }
        
        puppeteerAnimator.SetFloat(SpeedAnimator, speed);

        var dir = GetCameraMoveDirection(cameraTransform, _inputDirections);

        var rotation = Quaternion.LookRotation(dir);

        root.transform.rotation = rotation;

        var newPosition = root.transform.position;
        newPosition.y = _heightOffGround;
        newPosition += dir * (speed * Time.deltaTime);

        root.transform.position = newPosition;
    }

    /*private void FixedUpdate()
    {
        if (hasInput == false)
            return;

        var dir = GetCameraMoveDirection(cameraTransform, inputDirections);

        var rotation = Quaternion.LookRotation(dir);
        
        //root.MoveRotation(rotation);
        root.position += dir * (speed);
    }*/
    
    private void OnDisable()
    {
        PuppetRagdoll.OnRagdollActive -= OnRagdollActive;
    }

    //============================================================================================================//

    private void CheckGroundHeight(out float height)
    {
        height = 0f;
        
        var origin = root.transform.position;
        origin.y = 10f;

        if (Physics.Raycast(origin, Vector3.down, out var raycastHit, 20f, groundMask.value) == false)
            return;

        height = raycastHit.point.y + _groundHeightOffset;
    }
    
    private void OnRagdollActive(bool ragdollActive)
    {
        _ragdollActive = ragdollActive;
    }

    //============================================================================================================//

    private static void UpdateInputDirection(ref Vector2 direction)
    {
        direction = Vector2.zero;
        
        if (Input.GetKey(KeyCode.W))
        {
            direction.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction.y = -1;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            direction.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction.x = 1;
        }
    }

    private static Vector3 GetCameraMoveDirection(in Transform cameraTransform, in Vector2 inputDirection)
    {
        var cameraFwd = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        var cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        
        var finalForce = (cameraFwd * inputDirection.y) + (cameraRight * inputDirection.x);

        return finalForce;
    }
    
    //============================================================================================================//
}