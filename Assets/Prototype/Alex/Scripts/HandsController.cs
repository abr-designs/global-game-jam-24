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
    private float maxAccel = 10f; // Limit acceleration of thrown objects
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

        if (Input.GetKeyDown(KeyCode.Mouse0))
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
            return;
        }

        // First we determine where the mouse cursor is located
        
        var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Get facing direction of mouse from player
        Plane groundPlane = new Plane(Vector3.up, transform.position.y);
        if(!groundPlane.Raycast(screenPointToRay, out float enter))
        {
            _throwTarget = Vector3.zero;
            _throwDirection = Vector3.zero;
            return;
        }
        _throwTarget = screenPointToRay.GetPoint(enter);
        
        // First we need the rigidbody information
        // TODO -- should this information be cached to not query each frame?
        Rigidbody rb = _holdingObject.GetComponent<Rigidbody>();
        _throwIndicator.positionCount = throwIndicatorPoints + 1;
        Vector3 throwPoint = _holdingObject.GetThrowPoint();

        // TODO -- these should be configurable maybe? Possibly an angle/mass curve
        Vector3 upAmount = Vector3.up * Mathf.Max(rb.mass * .05f, .5f);
        _adjustedForce = Mathf.Min( rb.mass * maxAccel, force );

        _throwDirection = Vector3.Normalize(Vector3.ProjectOnPlane(_throwTarget - throwPoint, Vector3.up).normalized + upAmount);
        Vector3 startPos = throwPoint;
        Vector3 startVel = (_adjustedForce * _throwDirection) / rb.mass;

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
