using UnityEngine;

namespace InteractableObjects
{
    public class PropImpactOpenEvent : PropImpactEventBase
    {
        [SerializeField]
        private HingeJoint hingeJoint;

        [SerializeField]
        private float springTarget;

        protected override void TriggerImpactEvent(Collision collision)
        {
            var hingeJointSpring = hingeJoint.spring;
            hingeJointSpring.targetPosition = springTarget;

            hingeJoint.spring = hingeJointSpring;
        
            //TODO Add gold coin explosion

            //Turn off this event listener
            enabled = false;
        }
    }
}
