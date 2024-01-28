using UnityEngine;
using Utilities.ReadOnly;

namespace InteractableObjects
{
    [RequireComponent(typeof(PropObject))]
    public abstract class PropImpactEventBase : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private float largestRecorded;
        
        [SerializeField, Min(0)]
        private float minImpulse;

        private float MinImpulseSqr => minImpulse * minImpulse;
        
        private PropObject PropObject
        {
            get
            {
                if (_propObject)
                    return _propObject;

                _propObject = GetComponent<PropObject>();

                return _propObject;
            }
        }
        private PropObject _propObject;

        //============================================================================================================//

        private void OnEnable()
        {
            PropObject.OnImpulse += OnImpulse;
        }

        private void OnDisable()
        {
            PropObject.OnImpulse -= OnImpulse;
        }

        //============================================================================================================//

        private void OnImpulse(Collision collision)
        {
            var impulseMag = collision.impulse.magnitude;
            if(impulseMag > largestRecorded)
                largestRecorded = impulseMag;
            
            if (collision.impulse.sqrMagnitude < MinImpulseSqr)
                return;

            TriggerImpactEvent(collision);
        }

        protected abstract void TriggerImpactEvent(Collision collision);
    }
}
