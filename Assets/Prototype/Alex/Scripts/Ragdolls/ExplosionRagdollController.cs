using GameInput;
using UnityEngine;

public class ExplosionRagdollController : MonoBehaviour
{
    const float Radius = 2.5f;

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
    
    private bool _applyingForce;
    private float _forceToApply;

    private Vector3 _hitPoint;
    
    private bool _leftMousePressed;
    private bool _rightMousePressed;

    //============================================================================================================//

    private void OnEnable()
    {
        PuppetRagdoll.OnRagdollActive += OnRagdollActive;
        _rigidbodies = GetComponentsInChildren < Rigidbody>();
        
        GameInputDelegator.OnLeftClick += OnLeftClick;
        GameInputDelegator.OnRightClick += OnRightClick;
    }

    // Update is called once per frame
    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        
        if (Physics.Raycast(ray, out var raycastHit, 100, groundMask.value) == false)
            return;

        _hitPoint = raycastHit.point;
        sphereTransform.position = _hitPoint;
        Color color1 = Color.cyan;

        if (_leftMousePressed)
        {
            color1 = Color.cyan;
            _forceToApply = mult;
            _applyingForce = true;
        }
        else if (_rightMousePressed)
        {
            color1 = Color.red;
            _forceToApply = -mult * 10f;
            _applyingForce = true;
            //Offset up so doesn't stick to the ground
            _hitPoint += Vector3.up * Radius;
        }
        else
        {
            _applyingForce = false;
            color1.a = 0.1f;
            _forceToApply = 0f;
            material.SetColor("_BaseColor", color1);
            sphereTransform.localScale = Vector3.one * (Radius / 3f);
            return;
        }
        
        color1.a = 0.5f;
        material.SetColor("_BaseColor", color1);
        sphereTransform.localScale = Vector3.one * (Radius * 2f);
    }

    private void FixedUpdate()
    {
        if (_applyingForce == false)
            return;
        
        for (int i = 0; i < _rigidbodies.Length; i++)
        {
            _rigidbodies[i].AddExplosionForce(_forceToApply, _hitPoint, Radius, upForceMult);
        }
    }

    private void OnDisable()
    {
        PuppetRagdoll.OnRagdollActive -= OnRagdollActive;
        GameInputDelegator.OnLeftClick -= OnLeftClick;
        GameInputDelegator.OnRightClick -= OnRightClick;
    }

    //============================================================================================================//
    
    private void OnRagdollActive(bool ragdollActive)
    {
        _ragdollActive = ragdollActive;
    }
    
    //============================================================================================================//

    
    private void OnRightClick(bool pressed)
    {
        _rightMousePressed = pressed;
    }

    private void OnLeftClick(bool pressed)
    {
        _leftMousePressed = pressed;
    }
    
    //============================================================================================================//
}
