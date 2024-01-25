using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplosionRagdollController : MonoBehaviour
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

    // Update is called once per frame
    private void Update()
    {
        const float radius = 2.5f;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            return;
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        
        if (Physics.Raycast(ray, out var raycastHit, 100, groundMask.value) == false)
            return;

        sphereTransform.position = raycastHit.point;

        var color1 = Color.cyan;
        if (Input.GetKey(KeyCode.Mouse0) == false)
        {
            
            color1.a = 0.1f;
            material.SetColor("_Color", color1);
            sphereTransform.localScale = Vector3.one * (radius / 3f);
            return;
        }
        
        color1.a = 0.5f;
        material.SetColor("_Color", color1);
        sphereTransform.localScale = Vector3.one * (radius * 2f);

        for (int i = 0; i < _rigidbodies.Length; i++)
        {
            _rigidbodies[i].AddExplosionForce(mult, raycastHit.point, radius, 2f);
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
