using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StickyObject : MonoBehaviour
{
    private bool hasJoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnCollisionEnter(Collision other)
    {
        if (hasJoint)
            return;
        
        if (other.gameObject.CompareTag("Player") == false)
            return;

        hasJoint = true;

        var otherRigidBody = other.collider.attachedRigidbody;

        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = otherRigidBody;
        joint.enableCollision = false;
        joint.enablePreprocessing = false;
    }
}
