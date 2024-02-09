using System;
using Prototype.Alex.Scripts;
using Unity.Mathematics;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Serialization;
using VisualFX;

enum ThrowType
{
    DIRECT = 0,
    LOB = 1,
    CHARGE = 2,
    FIXED = 3
}

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
    private ThrowType throwType = ThrowType.DIRECT;

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
    [SerializeField, Range(10,100)]
    private int throwIndicatorPoints = 25;
    [SerializeField, Range(0.01f,0.25f)]
    private float throwIndicatorTimeStep = 0.1f;
    [SerializeField]
    private LayerMask throwLayerMask;
    [SerializeField]
    private AnimationCurve throwAngleCurve;
    
    [SerializeField]
    private AnimationCurve throwHeightCurve;

    // CHARGE THROW
        [SerializeField]
        private float minThrowSpeed = 10f;
        
        [SerializeField]
        private float maxThrowSpeed = 30f;
        private float throwSpeedRange => maxThrowSpeed - minThrowSpeed;
        
        [SerializeField]
        private float chargeTime = 3f;

        private float _chargeLevel = 0f;
        
    

    //============================================================================================================//
    private void Start()
    {
        if(_throwIndicator == null)
            _throwIndicator = Instantiate<LineRenderer>(throwIndicatorPrefab, transform);
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
        _objectInRange.Push(playerRootTransform.forward, _adjustedForce);
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
            _chargeLevel = 0;
            return;
        }

        Rigidbody rb = _holdingObject.GetComponent<Rigidbody>();
        _throwIndicator.positionCount = throwIndicatorPoints + 1;
        Vector3 throwPoint = _holdingObject.GetThrowPoint();

        // First we determine where the mouse cursor is located
        var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(throwType == ThrowType.LOB)
        {
            
            // Get facing direction of mouse from player
            Plane groundPlane = new Plane(Vector3.up, transform.position.y);
            if(!groundPlane.Raycast(screenPointToRay, out float enter))
            {
                _throwTarget = Vector3.zero;
                _throwDirection = Vector3.zero;
                return;
            }
            _throwTarget = screenPointToRay.GetPoint(enter);
            
            // How far the cursor is from the throw point
            Vector3 groundVector = Vector3.ProjectOnPlane(_throwTarget - throwPoint, Vector3.up);
            float groundDist = groundVector.magnitude;

            float theta = Mathf.Deg2Rad * throwAngleCurve.Evaluate(groundDist);
            Vector3 vel_x = groundVector.normalized * throwSpeed * Mathf.Cos(theta);
            Vector3 vel_y = Vector3.up * throwSpeed * Mathf.Sin(theta);
            Vector3 vel = vel_x + vel_y;

            _throwDirection = vel.normalized;
            _adjustedForce = rb.mass * vel.magnitude;

        } else if (throwType == ThrowType.DIRECT)
        {
            if(!Physics.Raycast(screenPointToRay, out RaycastHit raycastHit, 100f, throwLayerMask ) )
            {
                return;
            }
            _throwTarget = raycastHit.point;

            Vector3 throwVector =  _throwTarget - throwPoint;
            float heightAdjust = throwHeightCurve.Evaluate(throwVector.magnitude);

            _throwDirection = throwVector.normalized;
            _adjustedForce = throwSpeed * rb.mass;
        } else if (throwType == ThrowType.CHARGE)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _chargeLevel = Mathf.Clamp01(_chargeLevel + ((throwSpeedRange*(Time.deltaTime/chargeTime)) / throwSpeedRange) );
                Debug.Log($"ChargeLevel - {_chargeLevel}");
            }

            if(!Physics.Raycast(screenPointToRay, out RaycastHit raycastHit, 100f, throwLayerMask ) )
            {
                return;
            }
            _throwTarget = raycastHit.point;
            
            // TODO -- have angle change from straight to 45 degrees up?
            Vector3 throwVector = _throwTarget - throwPoint + Vector3.up * _chargeLevel * 5f;
            _throwDirection = throwVector.normalized;
            _adjustedForce = minThrowSpeed + throwSpeedRange * _chargeLevel * rb.mass;

        } else if(throwType == ThrowType.FIXED)
        {
            if(!Physics.Raycast(screenPointToRay, out RaycastHit raycastHit, 100f, throwLayerMask ) )
            {
                return;
            }
            _throwTarget = raycastHit.point;

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
            Debug.Log($"angle: {currentAngle} a0 {angle0} a1 {angle1} yOff: {heightOffset} ");
            _adjustedForce = throwSpeed * rb.mass;

        }

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
    
    }

    //============================================================================================================//

    private InteractableObject _objectInRange;
    private GameObject _currentHighlight;
    private void OnNewInteractableObject(InteractableObject objectInRange)
    {
        Debug.Log($"New interactable in range {objectInRange}");
        _objectInRange = objectInRange;
        

        // If character moved away from any objects
        if(objectInRange == null)
        {
            Debug.Log("No interactable in range");
            if(_currentHighlight)
            {
                Destroy(_currentHighlight);
                _currentHighlight = null;
            }
        } else {
            // Character moved near a new object - move highlight to object
            if(_currentHighlight == null)
                _currentHighlight = VFX.INTERACT_HIGHLIGHT.PlayAtLocation(objectInRange.transform.position, 1, true);
            _currentHighlight.transform.parent = _objectInRange.transform;
            _currentHighlight.transform.position = _objectInRange.transform.position;
        }

    }

    //----------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_throwTarget,.2f);
    }
}
