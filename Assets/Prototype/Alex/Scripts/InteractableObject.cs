using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private Vector3 localPivotOffset;

    private FixedJoint _joint;
    // Start is called before the first frame update

    public void Pickup(Vector3 worldPosition, Rigidbody attachTo)
    {
        transform.position = worldPosition + localPivotOffset;
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = attachTo;
    }

    public void Drop()
    {
        _joint.connectedBody = null;
        Destroy(_joint);
    }
}
