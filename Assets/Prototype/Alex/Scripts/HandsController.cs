using System;
using Prototype.Alex.Scripts;
using Unity.Mathematics;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Serialization;
using VisualFX;

public class HandsController : MonoBehaviour
{
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Rigidbody closestToLeftHand;
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private Rigidbody closestToRightHand;
    [SerializeField]
    private KeyCode interactKeyCode;

    [SerializeField]
    private float force;
    
    [SerializeField]
    private float throwSpeed = 20f; // Speed of object throwing
    
    private InteractableObject _holdingObject;
    [FormerlySerializedAs("playerpos")] [SerializeField]
    private Transform playerRootTransform;

    [SerializeField]
    private LineRenderer throwIndicatorPrefab;
    private LineRenderer _throwIndicator;
    
    [SerializeField]
    private TargetReticle reticlePrefab;
    private TargetReticle _reticle;
    
    [SerializeField, Range(10,100)]
    private int throwIndicatorPoints = 25;
    [SerializeField, Range(0.01f,0.25f)]
    private float throwIndicatorTimeStep = 0.1f;

    [SerializeField]
    private LayerMask throwLayerMask;

    private WASDRagdollController _playerController;
    

    //============================================================================================================//
    private void Start()
    {
        if(_throwIndicator == null)
            _throwIndicator = Instantiate<LineRenderer>(throwIndicatorPrefab, transform);
        if(_reticle == null)
            _reticle = Instantiate<TargetReticle>(reticlePrefab, transform);
        
        _playerController = GetComponent<WASDRagdollController>();
    }

    private void OnEnable()
    {
        ObjectInteractionController.OnNewInteractableObject += OnNewInteractableObject;
    }

    // Update is called once per frame
    private void Update()
    {
        // Show direction arrow
        UpdateTargetIndicator();

        if(_playerController.IsStunned)
            return;

        if (Input.GetKeyDown(interactKeyCode))
            ToggleObject();

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ThrowObject();
        }
    }

    private void OnDisable()
    {
        ObjectInteractionController.OnNewInteractableObject -= OnNewInteractableObject;
    }

    //============================================================================================================//
    private void ThrowObject()
    {
        if (_holdingObject)
        {
            //Instantiate(_throwIndicator); // clone throw line for debugging
            _holdingObject.Throw(_throwDirection /*leftHand.position*/, _adjustedForce);
            _holdingObject = null;
            return;
        }

        if (_objectInRange == null)
            return;


        //_holdingObject = _objectInRange;
        /*
        _objectInRange.Push(playerRootTransform.forward, force);
        var pushVFX = VFX.PUSH.PlayAtLocation(playerRootTransform.position);
        pushVFX.transform.parent = playerRootTransform;
        pushVFX.transform.rotation = playerRootTransform.rotation;
        */

    }

    private void ToggleObject()
    {
        if (_holdingObject)
        {
            _holdingObject.Drop();
            _holdingObject = null;
            return;
        }

        if (_objectInRange == null)
            return;


        _holdingObject = _objectInRange;
        //_holdingObject.Pickup(leftHand.position, closestToLeftHand);
        _holdingObject.Pickup(leftHand.position, leftHand);
    }

    private Vector3 _throwTarget;
    private Vector3 _throwDirection;
    private float _adjustedForce; // capping force so that objects will not fly too fast
    private void UpdateTargetIndicator()
    {
        if(!_holdingObject)
        {
            // disable indicator
            _throwIndicator.enabled = false;
            _reticle.gameObject.SetActive(false);
            return;
        }

        Rigidbody rb = _holdingObject.GetComponent<Rigidbody>();
        _throwIndicator.positionCount = throwIndicatorPoints + 1;
        Vector3 throwPoint = _holdingObject.GetThrowPoint();

        // First we determine where the mouse cursor is located
        var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(!Physics.Raycast(screenPointToRay, out RaycastHit raycastHit, 100f, throwLayerMask ) )
        {
            return;
        }
        _throwTarget = raycastHit.point;
        _reticle.transform.position = raycastHit.point;
        _reticle.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);

        Vector3 throwVector = _throwTarget - throwPoint;
        Vector3 groundVector = Vector3.ProjectOnPlane(throwVector,Vector3.up);
        float heightOffset = throwVector.y;
        float distance = groundVector.magnitude;

        bool targetInRange = ProjectileMath.LaunchAngle(throwSpeed, distance, heightOffset, Physics.gravity.magnitude, out float angle0, out float angle1 );
        float currentAngle = Mathf.PI / 4; // 45 degrees default
        if(targetInRange)
        {
            currentAngle = Mathf.Min(angle0,angle1);
        }

        // Update vectors with angle
        _throwDirection = Vector3.Normalize((groundVector.normalized * (Mathf.Cos(currentAngle) * throwSpeed)) + (Vector3.up * (Mathf.Sin(currentAngle) * throwSpeed)));
        //Debug.Log($"angle: {currentAngle} a0 {angle0} a1 {angle1} yOff: {heightOffset} ");
        _adjustedForce = throwSpeed * rb.mass;

        Vector3 startPos = throwPoint;
        Vector3 startVel = _throwDirection * (_adjustedForce / rb.mass);

        _throwIndicator.SetPosition(0, startPos);
        float time = 0f;
        for( int i=1; i <= throwIndicatorPoints; i++)
        {
            time += throwIndicatorTimeStep;
            Vector3 point = startPos + time * startVel;
            point.y = startPos.y + startVel.y * time + (Physics.gravity.y/2f * time * time);
            _throwIndicator.SetPosition(i,point);
        }

        // Show line
        _throwIndicator.enabled = true;
        _reticle.gameObject.SetActive(true);
    
    }

    //============================================================================================================//

    private InteractableObject _objectInRange;
    private InteractableObject _lastObjectInRange;
    private GameObject _currentHighlight;
    private void OnNewInteractableObject(InteractableObject objectInRange)
    {
        Debug.Log($"New interactable in range {objectInRange}");
        _lastObjectInRange = _objectInRange;
        _objectInRange = objectInRange;

        // Clear highlight from last object
        if(_lastObjectInRange != null)
        {
            //_lastObjectInRange.SetHighlight(false);
            _lastObjectInRange = null;
        }

        // If character moved away from any objects
        if(objectInRange == null)
        {
            //Debug.Log("No interactable in range");
            if(_currentHighlight)
            {
                Destroy(_currentHighlight);
                _currentHighlight = null;
            }
        } else {
            
            // Character moved near a new object - move highlight to object
            Bounds bounds = _objectInRange.GetBounds();
            float scale = Mathf.Max(new Vector2(bounds.extents.x,bounds.extents.z).magnitude * 2f, 1f);
            Vector3 pos = bounds.center;
            pos.y = Mathf.Max(bounds.min.y + 0.1f, 0.1f);
            
            if(_currentHighlight == null)
            {
                _currentHighlight = VFX.INTERACT_HIGHLIGHT.PlayAtLocation(pos, scale, true);
            } else {
                _currentHighlight.transform.position = pos;
                _currentHighlight.transform.localScale = Vector3.one * scale;
            }
            _currentHighlight.transform.parent = _objectInRange.transform;
            //_currentHighlight.transform.position = _objectInRange.transform.position;

            //_objectInRange.SetHighlight(true);
        }

    }

    //----------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_throwTarget,.2f);
    }
}
