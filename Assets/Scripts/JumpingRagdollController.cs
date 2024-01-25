using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpingRagdollController : MonoBehaviour
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

    [Min(0f)]
    public float mult = 10f;

    private Rigidbody[] _rigidbodies;

    [SerializeField, Min(0f)]
    private float time;
    private float timer;

    //============================================================================================================//

    private void OnEnable()
    {
        PuppetRagdoll.OnRagdollActive += OnRagdollActive;
        _rigidbodies = GetComponentsInChildren < Rigidbody>();
    }



    // Start is called before the first frame update
    private void Start()
    {
        //CheckGroundHeight(out _groundHeightOffset);
        //_groundHeightOffset *= 0.95f;
    }

    // Update is called once per frame
    private void Update()
    {
        const float radius = 2.5f;

        if (timer < time)
        {
            timer += Time.deltaTime;
            return;
        }

        timer = 0f;

        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        
        if (Physics.Raycast(ray, out var raycastHit, 100, groundMask.value) == false)
            return;
        
        Debug.DrawLine(ray.origin, raycastHit.point, Color.green, 2f);


        var dir = Vector3.ProjectOnPlane(raycastHit.point - root.transform.position, Vector3.up);
        var normalizer = dir.normalized;

        var point = root.transform.position - (normalizer);
       
        Debug.DrawRay(point, Vector3.up, Color.red, 1f);

        for (int i = 0; i < _rigidbodies.Length; i++)
        {
            _rigidbodies[i].AddExplosionForce(mult, point, radius, 2f);
        }

    }

    
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