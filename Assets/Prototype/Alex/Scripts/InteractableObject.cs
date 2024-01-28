using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private Vector3 localPivotOffset;
    [SerializeField]
    private LayerMask groundMask;
    private FixedJoint _joint;
    // Start is called before the first frame update
    private Rigidbody rb;
    private float InitialMass;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        InitialMass = rb.mass;
    }
    public void Pickup(Vector3 worldPosition, Rigidbody attachTo)
    {
        rb.mass = 0;
        transform.position = worldPosition + localPivotOffset;
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = attachTo;
        Debug.Log("Picked");

    }

    public void Drop()
    {
        rb.mass = InitialMass;
        _joint.connectedBody = null;
        Destroy(_joint);
    }

    public void Throw(Vector3 fromvec, float force)
    {
        rb.mass = InitialMass;
        _joint.connectedBody = null;
        Destroy(_joint);
        var tovec = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(tovec, out var raycastHit, 100, groundMask.value) == false)
            return;

        var point = raycastHit.point;
        //dest - origin
        Vector3 thrust = (point - fromvec + Vector3.up).normalized;
        float thrustMag = force;
        rb.AddForce(thrust * thrustMag, ForceMode.Impulse);

        Debug.DrawLine(fromvec, point, Color.black, 5.0f, true);

    }

    public void Swat(Vector3 dir, float force)
    {
        Vector3 thrust = dir * force;
        rb.AddForce(thrust + Vector3.up * 0.2f, ForceMode.Impulse);

    }
}
