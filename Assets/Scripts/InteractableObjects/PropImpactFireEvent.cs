using UnityEngine;
using VisualFX;

namespace InteractableObjects
{
    public class PropImpactFireEvent : PropImpactEventBase
    {
        private GameObject _flamesVfx;
    
        protected override void TriggerImpactEvent(Collision collision)
        {
            //TODO Need to add sparks effect
            _flamesVfx = VFX.FLAMES.PlayAtLocation(transform.position, keepAlive: true);

            _flamesVfx.transform.SetParent(transform, true);
            enabled = false;
        }
    }
}
