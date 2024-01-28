using UnityEditor;
using UnityEngine;

public class RagdollHelper : MonoBehaviour
{
    [SerializeField]
    private Rigidbody parentBody;
    [SerializeField]
    private Vector3 boneAxis;
    [SerializeField]
    private Vector3 secondaryAxis;
    [SerializeField]
    private Transform bonePivotPoint;
    [SerializeField]
    private Transform boneTargetPoint;

    [SerializeField]
    private float radius;
    
#if UNITY_EDITOR

    [ContextMenu("Generate Bone")]
    private void CreateCapsuleBone()
    {
        var rigidbody = bonePivotPoint.GetComponent<Rigidbody>();
        var collider = bonePivotPoint.GetComponent<CapsuleCollider>();
        var joint = bonePivotPoint.GetComponent<ConfigurableJoint>();

        if (rigidbody == null) rigidbody = bonePivotPoint.gameObject.AddComponent<Rigidbody>();
        if (collider == null) collider = bonePivotPoint.gameObject.AddComponent<CapsuleCollider>();
        if (joint == null) joint = bonePivotPoint.gameObject.AddComponent<ConfigurableJoint>();

        //------------------------------------------------//
        collider.radius = radius;
        collider.center =
            bonePivotPoint.InverseTransformPoint(Vector3.Lerp(bonePivotPoint.position, boneTargetPoint.position, 0.5f));
        collider.height = (bonePivotPoint.position - boneTargetPoint.position).magnitude;
        //From: https://discussions.unity.com/t/capsule-collider-how-direction-change-to-z-axis-c/225672
        //It takes an int, so 0 = x, 1 = y, 2 = z
        if (Mathf.Abs(boneAxis.x) > 0)
            collider.direction = 0;
        else if (Mathf.Abs(boneAxis.y) > 0)
            collider.direction = 1;
        else if (Mathf.Abs(boneAxis.z) > 0)
            collider.direction = 2;

        //------------------------------------------------//

        joint.connectedBody = parentBody;
        joint.axis = boneAxis;
        joint.secondaryAxis = secondaryAxis;
        
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        joint.rotationDriveMode = RotationDriveMode.Slerp;

        var jointSlerpDrive = joint.slerpDrive;
        jointSlerpDrive.positionSpring = 1000f;
        joint.slerpDrive = jointSlerpDrive;


        EditorUtility.SetDirty(this);
    }
#endif
}
