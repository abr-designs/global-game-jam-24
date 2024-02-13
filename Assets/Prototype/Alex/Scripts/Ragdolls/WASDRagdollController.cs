using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using GameInput;
using Gameplay;
using UnityEngine;
using VisualFX;

public class WASDRagdollController : MonoBehaviour
{
    private static readonly int SpeedAnimator = Animator.StringToHash("Speed");
    
    [SerializeField]
    private Animator puppeteerAnimator;

    [SerializeField]
    private Rigidbody root;
    private ConfigurableJoint[] _playerJoints;

    private Transform _cameraTransform;
    [SerializeField, Min(0)]
    private float speed;

    private Vector2 _inputDirections;
    private bool _hasInput;

    private bool _ragdollActive;

    [SerializeField]
    private LayerMask groundMask;

    private float _groundHeightOffset;
    private float _heightOffGround;

    [SerializeField]
    private Vector3 minRoomPosition;
    [SerializeField]
    private Vector3 maxRoomPosition;
    
    //============================================================================================================//

    private void OnEnable()
    {
        GameInputDelegator.OnMovementChanged += OnMovementChanged;
        PuppetRagdoll.OnRagdollActive += OnRagdollActive;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        CheckGroundHeight(out _groundHeightOffset);
        _groundHeightOffset *= 0.95f;

        _playerJoints = GetComponentsInChildren<ConfigurableJoint>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_ragdollActive)
        {
            // Set kinematic root to the average of all joints
            Vector3 average = Vector3.zero;
            foreach(ConfigurableJoint joint in _playerJoints)
            {
                average += joint.transform.position;
            }
            average = average / _playerJoints.Length;
            root.transform.position = average;

            return;
        }

        if(_isStunned)
            return;
        
        CheckGroundHeight(out _heightOffGround);

        _hasInput = _inputDirections != Vector2.zero;

        if (_hasInput == false)
        {
            puppeteerAnimator.SetFloat(SpeedAnimator, 0f);
            return;
        }
        
        puppeteerAnimator.SetFloat(SpeedAnimator, speed);

        var dir = GetCameraMoveDirection(_cameraTransform, _inputDirections);

        var rotation = Quaternion.LookRotation(dir);

        root.transform.rotation = rotation;

        var newPosition = root.transform.position;
        newPosition.y = _heightOffGround;
        newPosition += dir * (speed * Time.deltaTime);

        root.transform.position = ClampPositionToBounds(newPosition, minRoomPosition, maxRoomPosition);
    }
    
    private void OnDisable()
    {
        PuppetRagdoll.OnRagdollActive -= OnRagdollActive;
        GameInputDelegator.OnMovementChanged -= OnMovementChanged;
    }

    //============================================================================================================//

    private void CheckGroundHeight(out float height)
    {
        height = 0f;
        
        var origin = root.transform.position;
        origin.y = 10f;

        if (Physics.Raycast(origin, Vector3.down, out var raycastHit, 20f, groundMask.value) == false)
        {
            Debug.DrawRay(origin, Vector3.down * 100, Color.red);
            return;
        }
        Debug.DrawLine(origin, raycastHit.point, Color.green);

        height = raycastHit.point.y + _groundHeightOffset;
    }
    
    private void OnRagdollActive(bool ragdollActive)
    {
        _ragdollActive = ragdollActive;
    }

    /// <summary>
    /// Does not apply clamp to Y position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="mult"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector3 ClampPositionToBounds(Vector3 position, Vector3 min, Vector3 max, float mult = 1f)
    {
        position.x = Math.Clamp(position.x, min.x, max.x);
        //position.y = Math.Clamp(position.y, min.y, max.y);
        position.z = Math.Clamp(position.z, min.z, max.z);

        return position;
    }

    //============================================================================================================//

    private void OnMovementChanged(Vector2 movementInput)
    {
        _inputDirections = movementInput;
    }

    private static Vector3 GetCameraMoveDirection(in Transform cameraTransform, in Vector2 inputDirection)
    {
        var cameraFwd = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        var cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        
        var finalForce = (cameraFwd * inputDirection.y) + (cameraRight * inputDirection.x);

        return finalForce;
    }
    
    private bool _isStunned = false;
    public bool IsStunned => _isStunned;

    public void StunPlayer(float stunTime)
    {
        // Can't stun player twice
        if(_isStunned) return;
        
        StartCoroutine(StunTimerCoroutine(stunTime));
    }

    private IEnumerator StunTimerCoroutine(float waitTime)
    {
        _isStunned = true;
        PuppetRagdoll player = GetComponent<PuppetRagdoll>();
        //GameInputDelegator.LockInputs = true;
        player.EnableRagdoll(true);
        player.transform.FindObjectWithName("KinematicRoot", out var root);

        var stunVFX = VFX.STUNNED_STARS.PlayAtLocation(root.transform.position + Vector3.up, 1, true);
        stunVFX.transform.parent = root.transform;

        yield return new WaitForSeconds(waitTime);

        player.EnableRagdoll(false);
        _isStunned = false;

        Destroy(stunVFX);
        
    }

    //============================================================================================================//

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var points = new[]
        {
            /*tl*/new Vector3(minRoomPosition.x, maxRoomPosition.y, maxRoomPosition.z),
            /*tr*/maxRoomPosition,
            /*br*/new Vector3(maxRoomPosition.x, minRoomPosition.y, minRoomPosition.z),
            /*bl*/minRoomPosition
        };
        
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLineStrip(points, true);

        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawWireSphere(points[i], 0.1f);
        }

        if (root.transform == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(root.transform.position, 0.2f);
    }
#endif
}