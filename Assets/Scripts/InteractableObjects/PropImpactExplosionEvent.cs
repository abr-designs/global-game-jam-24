using System.Collections;
using Audio;
using Audio.SoundFX;
using UnityEngine;
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

        [SerializeField]private VFX vfxOnExplosion;
        [SerializeField]private SFX sfxOnExplosion;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private Vector3 localVfxPosition;
        
        private GameObject _flamesVfx; 

        protected override void TriggerImpactEvent(Collision _)
        {
            if (_triggered)
                return;

            StartCoroutine(ExplosionTimerCoroutine(explosionTimer));
            
            //TODO Need to add sparks effect
            _flamesVfx = VFX.FLAMES.PlayAtLocation(transform.TransformPoint(localVfxPosition), keepAlive: true);

            _flamesVfx.transform.SetParent(transform, true);
            _triggered = true;
        }
        

        private IEnumerator ExplosionTimerCoroutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask.value);

            bool didHitPlayer = false;
            foreach (var collider in colliders)
            {
                var targetRb = collider.attachedRigidbody;
                
                if(targetRb == null)
                    continue;

                if(!didHitPlayer && collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Explosion hit player!");
                    didHitPlayer = true;
                    var player = collider.GetComponentInParent<WASDRagdollController>();
                    player.StunPlayer(3f);
                }

                //targetRb.isKinematic = false;
                Vector3 force = Vector3.Normalize(collider.transform.position - transform.position + Vector3.up * 1f) * explosionForce;
                // Trigger any props
                if(collider.gameObject.TryGetComponent<PropObject>(out PropObject prop))
                {
                    // TODO -- maybe add impulse based on distance?
                    prop.ApplyImpact(PropImpactType.EXPLOSION, force * Time.fixedDeltaTime);
                }

                collider.attachedRigidbody?.AddForce(force, ForceMode.Force);
                //collider.attachedRigidbody?.AddExplosionForce(explosionForce, transform.position, 0 /*explosionRadius*/, 0.5f);
        
            }
            
            // Score the object
            var _source = GetComponent<PropObject>();
            _source?.TriggerScore();

            //TODO Need to add the gibs
            vfxOnExplosion.PlayAtLocation(transform.position, 1f);
            VFX.RED_CLOUD.PlayAtLocation(transform.position, explosionRadius * 1.5f);
            sfxOnExplosion.PlaySoundAtLocation(transform.position);
            Destroy(gameObject);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.TransformPoint(localVfxPosition), 0.05f);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }

#endif
    }
}
