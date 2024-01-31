using UnityEngine;
using VisualFX;

namespace InteractableObjects
{
    public class PropImpactOpenEvent : PropImpactEventBase
    {
        [SerializeField]
        private HingeJoint hingeJoint;

        [SerializeField]
        private float springTarget;

        [SerializeField]
        private GameObject[] spawnables;

        [SerializeField]
        private Vector3 spawnableOffset;

        protected override void TriggerImpactEvent(Collision collision)
        {
            var hingeJointSpring = hingeJoint.spring;
            hingeJointSpring.targetPosition = springTarget;

            hingeJoint.spring = hingeJointSpring;
        
            VFX.EXPLOSION_COINS.PlayAtLocation(transform.position);

            // Load any spawnables from open
            foreach(GameObject obj in spawnables)
            {
                GameObject newObj = Instantiate(obj,transform.position + spawnableOffset, transform.rotation, transform.parent);

                // Apply force
                foreach(Transform child in newObj.transform)
                {
                    child.parent = transform.parent;
                    child.GetComponent<Rigidbody>().AddForce(collision.impulse, ForceMode.Impulse);
                }

            }

            //Turn off this event listener
            enabled = false;
        }
    }
}
