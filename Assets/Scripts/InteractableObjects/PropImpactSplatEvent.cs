using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace InteractableObjects
{
    public class PropImpactSplatEvent : PropImpactEventBase
    {
        [SerializeField]private DecalProjector decalPrefab;
        [SerializeField]private Material decalMaterial;

        [SerializeField, Min(0f)]
        private float minSize;
        [SerializeField, Min(0f)]
        private float maxSize;

        
        private float _spawnWaitTime = 5f;
        private float _lastSpawned;
        
        protected override void TriggerImpactEvent(Collision collision)
        {
            if (Time.time - _lastSpawned <= _spawnWaitTime)
                return;
            
            var decal = Instantiate(decalPrefab);
            decal.material = decalMaterial;

            var firstContact = collision.contacts[0];
            var myColliderPos = firstContact.thisCollider.transform.position;
            var dir = (myColliderPos - firstContact.point).normalized; 
            
            
            decal.transform.position = firstContact.point + Vector3.up;
            decal.transform.forward = Vector3.down;

            decal.size = new Vector3(
                Random.Range(minSize, maxSize),
                Random.Range(minSize, maxSize),
                5f
                );
        }
    }
}