using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Prototype.Alex.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField]
        private Vector3 localPivotOffset;
        [SerializeField]
        private LayerMask groundMask;
        private FixedJoint _joint;
        private Rigidbody _rigidbody;
        private float _initialMass;

        private Transform _target;        

        //Unity Functions
        //============================================================================================================//

        // Start is called before the first frame update
        private void Start()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            _initialMass = _rigidbody.mass;

            Assert.IsTrue(groundMask > 0, $"Interactable {name} has no ground mask set!");
        }

        //============================================================================================================//

        private void Update() 
        {
            if(_target)
            {
                // TODO -- make the follow speeds configurable for different objects
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _target.transform.rotation, 1000f * Time.deltaTime);
                transform.position = _target.position + localPivotOffset;
                // From new position we re-adjust for ground clipping
                transform.position = GetThrowPoint();
            }
        }

        public void Pickup(Vector3 worldPosition, Transform attachTo)
        {
            // Set the hierarchy into the physics system
            // This is to ensure that we only have the prop as kinematic in hand and the rest are simulated
            if(TryGetComponent<PropObject>(out PropObject prop))
            {   
                prop.PlayerTriggerProp();
            }

            // New plan
            // Set the object to kinematic and put on a different layer
            // Have the object follow the target in update
            _rigidbody.isKinematic = true;
            _rigidbody.detectCollisions = false;
            _target = attachTo;

        }

        public void Drop()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.detectCollisions = true;
            _target = null;
        }

        public void Throw(Vector3 throwDirection, float launchForce)
        {
            // Set object to a layer that won't collide with the player;
            gameObject.layer = LayerMask.NameToLayer("throw");

            // First we need to move object outside of any groundmask colliders if possible
            transform.position = GetThrowPoint();

            // Set the object back to the physics system
            Drop();

            var launchDirection = throwDirection.normalized;
            _rigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

        }

        public void Push(Vector3 dir, float force)
        {
            if (_rigidbody.isKinematic == true)
            {
                _rigidbody.isKinematic = false;
            }
            Vector3 thrust = dir * force;
            _rigidbody.AddForce(thrust + Vector3.up * 0.2f, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Reset layer
            if(gameObject.layer == LayerMask.NameToLayer("throw"))
            {
                gameObject.layer = LayerMask.NameToLayer("interactable");
                Debug.Log("Throw object layer reset");
            }
                
        }

        private Bounds _currentBounds;
        public Bounds GetBounds()
        {
            if(_currentBounds == null)
            {
                _currentBounds = new Bounds(transform.position,Vector3.zero);
            }
            _currentBounds.center = transform.position;
            _currentBounds.size = Vector3.zero;

            // First we need to move object outside of any groundmask colliders if possible
            Collider[] colliders = GetComponentsInChildren<Collider>();
            // Build a bounding box from all nested colliders
            // TODO -- is this already somewhere in the physics system?
            foreach(Collider collider in colliders)
            {
                //if(collider.attachedRigidbody.isKinematic == true)
                    _currentBounds.Encapsulate(collider.bounds);
            }
            return _currentBounds;
        }

        // This will return the adjusted throw point when an object is released
        // It uses the object bounds to take into account moving it above the ground
        private const float groundMinOffset = 0.05f;
        public Vector3 GetThrowPoint() 
        {
            Bounds bounds = GetBounds();

            // Determine how much we need to move the box above the ground
            float offset = Mathf.Abs(Mathf.Min(bounds.min.y - groundMinOffset, 0));


            //Debug.Log($"Offset for throw {offset}, {bounds.min} {bounds.extents.y}");
            //Debug.DrawLine(transform.position, transform.position + Vector3.up * offset, Color.green, 5.0f);
            Vector3 result = transform.position + (Vector3.up * offset);
            //result.y = Mathf.Min(result.y,maxGroundHeight);
            return result;
        }

    }
}
