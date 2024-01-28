using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using VisualFX;

namespace InteractableObjects
{
    public class PropImpactExplosionEvent : PropImpactEventBase
    {
        private bool _triggered;
        [SerializeField, Min(0f)]
        private float explosionTimer;
        [SerializeField, Min(0f)]
        private float explosionForce;
        [SerializeField, Min(0f)]
        private float explosionRadius;

        [SerializeField]
        private LayerMask layerMask;
        
        protected override void TriggerImpactEvent(Collision _)
        {
            if (_triggered)
                return;

            StartCoroutine(ExplosionTimerCoroutine(explosionTimer));
            
            //TODO Need to add sparks effect

            _triggered = true;
        }

        private IEnumerator ExplosionTimerCoroutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask.value);

            foreach (var collider in colliders)
            {
                var targetRb = collider.attachedRigidbody;
                
                if(targetRb == null)
                    continue;


                targetRb.isKinematic = false;
                collider.attachedRigidbody?.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            
            //TODO Need to add the gibs
            VFX.EXPLOSION_BARREL.PlayAtLocation(transform.position, 3f);
            Destroy(gameObject);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }

#endif
    }
}
