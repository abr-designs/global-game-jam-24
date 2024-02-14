using Audio;
using Audio.SoundFX;
using Levels;
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

        private const float SpawnWaitTime = 5f;
        private float _lastSpawned;
        
        protected override void TriggerImpactEvent(Collision collision)
        {
            if (Time.time - _lastSpawned <= SpawnWaitTime)
                return;
            
            var _source = GetComponent<PropObject>();
            _source?.TriggerScore();

            SFX.SQUELCH.PlaySoundAtLocation(transform.position);
            
            var decal = Instantiate(decalPrefab, LevelLoader.CurrentLevelController.transform, true);
            decal.material = decalMaterial;

            var firstContact = collision.contacts[0];
            //var myColliderPos = firstContact.thisCollider.transform.position;
            
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