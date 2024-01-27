using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VisualFX;

public class ExplosionRagdollController : MonoBehaviour
{
    const float radius = 2.5f;

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
    [Min(0f)]
    public float upForceMult = 2f;

    private Rigidbody[] _rigidbodies;

    [SerializeField]
    private Transform sphereTransform;
    [SerializeField]
    private Material material;

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

    private bool applyingForce;
    private float _forceToApply;

    private Vector3 hitPoint;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            return;
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        
        if (Physics.Raycast(ray, out var raycastHit, 100, groundMask.value) == false)
            return;

        hitPoint = raycastHit.point;
        sphereTransform.position = hitPoint;
        Color color1 = Color.cyan;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            VFX.EXPLOSION.PlayAtLocation(hitPoint);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            color1 = Color.cyan;
            _forceToApply = mult;
            applyingForce = true;
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            color1 = Color.red;
            _forceToApply = -mult * 10f;
            applyingForce = true;
            //Offset up so doesn't stick to the ground
            hitPoint += Vector3.up * radius;
        }
        else
        {
            applyingForce = false;
            color1.a = 0.1f;
            _forceToApply = 0f;
            material.SetColor("_BaseColor", color1);
            sphereTransform.localScale = Vector3.one * (radius / 3f);
            return;
        }
        
        color1.a = 0.5f;
        material.SetColor("_BaseColor", color1);
        sphereTransform.localScale = Vector3.one * (radius * 2f);
    }

    private void FixedUpdate()
    {
        if (applyingForce == false)
            return;
        
        for (int i = 0; i < _rigidbodies.Length; i++)
        {
            _rigidbodies[i].AddExplosionForce(_forceToApply, hitPoint, radius, upForceMult);
        }
    }

    private void MouseMove()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var raycastHit, 100, groundMask.value) == false)
            return;


        Debug.DrawLine(ray.origin, raycastHit.point, Color.green, 1f);
        var dif = root.transform.position - raycastHit.point;
        root.transform.position = Vector3.MoveTowards(root.transform.position,
            raycastHit.point,
            dif.magnitude * mult * Time.deltaTime);
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
