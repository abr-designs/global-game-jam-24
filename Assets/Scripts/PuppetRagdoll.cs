using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PuppetRagdoll : MonoBehaviour
{
    [Serializable]
    private class JointConnectData
    {
        public string name;
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
        for (int i = 0; i < jointMirrors.Length; i++)
        {
            jointMirrors[i].MirrorRotation();
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

