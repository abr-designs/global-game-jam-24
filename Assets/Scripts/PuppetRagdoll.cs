using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PuppetRagdoll : MonoBehaviour
{
    public static event Action<bool> OnRagdollActive; 
    [Serializable]
    private class JointConnectData
    {
        public string name;
        public bool enabled = true;
        public ConfigurableJoint joint;
        public Transform animationSkeletonReference;
        private Quaternion cachedRotation;

        public void Init()
        {
	        cachedRotation = joint.transform.localRotation;
        }

        public void MirrorRotation()
        {
            joint.SetTargetRotationLocal(animationSkeletonReference.localRotation, cachedRotation);
            //joint.targetRotation = animationSkeletonReference.localRotation;
        }
    }

    [SerializeField]
    private JointConnectData[] jointMirrors;

    [SerializeField]
    private bool ragdollActive;

    //============================================================================================================//
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < jointMirrors.Length; i++)
        {
            jointMirrors[i].Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            EnableRagdoll(!ragdollActive);
        
        if (ragdollActive)
            return;
        
        for (int i = 0; i < jointMirrors.Length; i++)
        {
            if (jointMirrors[i].enabled == false)
                continue;
            
            jointMirrors[i].MirrorRotation();
        }
    }

    [SerializeField]
    private ConfigurableJoint hipsJoint;
    private void EnableRagdoll(bool isActive)
    {
        const float targetSpring = 1000f;
        const float dampening = 100f;

        ragdollActive = isActive;
        
        OnRagdollActive?.Invoke(ragdollActive);
        
        hipsJoint.xMotion = isActive ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
        hipsJoint.yMotion = isActive ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
        hipsJoint.zMotion = isActive ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
        
        
        for (int i = 0; i < jointMirrors.Length; i++)
        {
            var joint = jointMirrors[i].joint;

            var jointSlerpDrive = joint.slerpDrive;
            jointSlerpDrive.positionSpring = isActive ? 0f : targetSpring;
            jointSlerpDrive.positionDamper = isActive ? 0f : dampening;

            joint.slerpDrive = jointSlerpDrive;
        }
    }

    //============================================================================================================//

    [SerializeField]
    private GameObject puppeteerReference;
    [ContextMenu("Find Joints")]
    [Conditional("UNITY_EDITOR")]
    private void FindJoints()
    {
        //------------------------------------------------//
        bool FindObjectWithName(Transform parent, in string name, out Transform foundTransform)
        {
            foundTransform = null;
            var childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);

                if (child.gameObject.name.Equals(name))
                {
                    foundTransform = child;
                    return true;
                }

                if (FindObjectWithName(child, in name, out foundTransform))
                {
                    return true;
                }
            }

            return false;
        }
        
        //------------------------------------------------//
        
        var joints = GetComponentsInChildren<ConfigurableJoint>();

        jointMirrors = new JointConnectData[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            var name = joints[i].gameObject.name;
            FindObjectWithName(puppeteerReference.transform, in name, out var foundTransform);
            
            jointMirrors[i] = new JointConnectData
            {
                name = name,
                joint = joints[i],
                animationSkeletonReference = foundTransform
            };
        }
    }

    [ContextMenu("Convert Character Joints")]
    [Conditional("UNITY_EDITOR")]
    private void ConvertCharacterJoints()
    {
        var characterJoints = GetComponentsInChildren<CharacterJoint>();

        for (int i = 0; i < characterJoints.Length; i++)
        {
            var charJoint = characterJoints[i];
            var gameObject = charJoint.gameObject;

            var conJoint = gameObject.AddComponent<ConfigurableJoint>();

            conJoint.connectedBody = charJoint.connectedBody;
            conJoint.axis = charJoint.axis;
            conJoint.connectedAnchor = charJoint.connectedAnchor;
            conJoint.secondaryAxis = charJoint.swingAxis;
            
            /*onJoint.lowAngularXLimit = charJoint.lowTwistLimit;
            conJoint.highAngularXLimit = charJoint.highTwistLimit;

            conJoint.angularYLimit = charJoint.swing1Limit;
            conJoint.angularZLimit = charJoint.swing2Limit;*/

            conJoint.xMotion = ConfigurableJointMotion.Limited;
            conJoint.yMotion = ConfigurableJointMotion.Limited;
            conJoint.zMotion = ConfigurableJointMotion.Limited;
            
            conJoint.angularXMotion = ConfigurableJointMotion.Limited;
            conJoint.angularYMotion = ConfigurableJointMotion.Limited;
            conJoint.angularZMotion = ConfigurableJointMotion.Limited;

            conJoint.rotationDriveMode = RotationDriveMode.Slerp;
            conJoint.enablePreprocessing = false;

            var jointDrive = new JointDrive
            {
                positionSpring  = 500,
                positionDamper = 0,
                maximumForce = Mathf.Infinity
            };
            conJoint.slerpDrive = jointDrive;
            conJoint.angularYZDrive = jointDrive;
            conJoint.angularXDrive = jointDrive;
            
            DestroyImmediate(charJoint);
        }
    }
    
    //============================================================================================================//
}

