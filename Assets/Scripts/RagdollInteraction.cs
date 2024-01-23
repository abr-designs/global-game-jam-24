using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RagdollInteraction : MonoBehaviour
{
    private enum MODE
    {
        NONE,
        WASD,
        WASD_DRIVER,
        DRAG,
        LAUNCH
    }

    [SerializeField]
    private MODE currentMode;
    
    [SerializeField]
    private LayerMask raycastMask;

    [SerializeField, Header("WASD Properties")]
    private Rigidbody targetRigidbody;
    [SerializeField, Min(0)]
    private float force;
    [SerializeField]
    private ForceMode forceMode = ForceMode.VelocityChange;
    
    [SerializeField, Header("WASD DriverProperties")]
    private Rigidbody driverRigidBody;
    [SerializeField]
    private Transform driverTransform;
    [SerializeField, Min(0f)] 
    private float driverRadius = 1f;
    [SerializeField, Min(0)]
    private float driveForce;

    [SerializeField, Min(1f)] private float driverJointForceMult = 1f;
    [SerializeField, Min(0f)]private float rotationSpeed;
    [SerializeField]
    private Rigidbody hipsRigidbody;
    [SerializeField]
    private Rigidbody headRigidbody;


    private Transform _cameraTransform;
    
    // Start is called before the first frame update
    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        //Reset the scene
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
        
        switch (currentMode)
        {
            case MODE.NONE:
                break;
            case MODE.WASD:
                WASDRagdoll();
                break;
            case MODE.WASD_DRIVER:
                WASDDriverRagdoll();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    //============================================================================================================//

    private void WASDRagdoll()
    {
        var cameraFwd = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
        var cameraRight = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;
        
        Vector2 forceToApply = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            forceToApply.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forceToApply.y = -1;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            forceToApply.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            forceToApply.x = 1;
        }

        var finalForce = (cameraFwd * forceToApply.y) + (cameraRight * forceToApply.x);
        
        targetRigidbody.AddForce(finalForce * force, forceMode);
    }

    private Vector3 forward;
    private void WASDDriverRagdoll()
    {
        var cameraFwd = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
        var cameraRight = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;
        
        Vector2 forceToApply = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            forceToApply.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forceToApply.y = -1;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            forceToApply.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            forceToApply.x = 1;
        }

        
        var finalForce = (cameraFwd * forceToApply.y) + (cameraRight * forceToApply.x);
        
        forward = finalForce;
        driverRigidBody.velocity = finalForce * driveForce;
    }

    private void FixedUpdate()
    {
        if (currentMode != MODE.WASD_DRIVER)
            return;

        var driverPos = driverRigidBody.position;
        var targetHeadPos = driverPos + (Vector3.up * driverRadius);

        var currentHeadPos = headRigidbody.position;
        var currentHipsPos = hipsRigidbody.position;

        var posDifHead = targetHeadPos - currentHeadPos;
        var posDifHips = driverPos - currentHipsPos;

        headRigidbody.velocity = posDifHead.normalized * (posDifHead.magnitude * driverJointForceMult);
        hipsRigidbody.velocity = posDifHips.normalized * (posDifHips.magnitude * driverJointForceMult);

        headRigidbody.angularVelocity = Vector3.zero;
        hipsRigidbody.angularVelocity = Vector3.zero;
        //RotateTowards(headRigidbody, forward);
        //RotateTowards(hipsRigidbody, forward);
    }

    private void RotateTowards(Rigidbody rigidbody, Vector3 fwd)
    {
        var targetRot = Quaternion.FromToRotation(rigidbody.transform.forward, fwd);
        var currentRot = rigidbody.rotation;
        var newRotation = Quaternion.RotateTowards(currentRot, targetRot, rotationSpeed * Time.fixedDeltaTime);

        rigidbody.MoveRotation(newRotation);
    }

    //============================================================================================================//

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(driverTransform.position, driverRadius);
    }
}
