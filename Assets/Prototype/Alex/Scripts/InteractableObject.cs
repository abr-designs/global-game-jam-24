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
            }
        }

        public void Pickup(Vector3 worldPosition, Transform attachTo)
        {
            // New plan
            // Set the object to kinematic and put on a different layer
            // Have the object follow the target in update
            _rigidbody.isKinematic = true;
            _rigidbody.detectCollisions = false;
            _target = attachTo;


            // OLD CODE BELOW
            /*
            if (_rigidbody.isKinematic == true)
            {
                _rigidbody.isKinematic = false;
            }
            _rigidbody.mass = 0;

            transform.position = worldPosition + localPivotOffset;
            transform.localRotation = attachTo.rotation;
            _joint = gameObject.AddComponent<FixedJoint>();
            _joint.connectedBody = attachTo;
            */
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


            // First look for a valid directional point
            var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenPointToRay, out var raycastHit, 100, groundMask.value) == false)
            {
                Debug.Log($"Throw raycast hit nothing!!!!");
                // Set the object back to the physics system
                Drop();
                return;
            }

            // First we need to move object outside of any groundmask colliders if possible
            Collider[] colliders = GetComponentsInChildren<Collider>();
            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            // Build a bounding box from all nested colliders
            // TODO -- is this already somewhere in the physics system?
            foreach(Collider collider in colliders)
            {
                bounds.Encapsulate(collider.bounds);
            }
            // Determine how much we need to move the box above the ground
            float offset = Mathf.Abs(Mathf.Min(bounds.min.y, 0)) + 0.1f; // add a little bit extra to avoid overlap
            Debug.Log($"Offset for throw {offset}, {bounds.min} {bounds.extents.y}");
            Debug.DrawLine(transform.position, transform.position + Vector3.up * offset, Color.green, 5.0f);
            transform.Translate(Vector3.up * offset, Space.World);

            // Set the object back to the physics system
            Drop();

            var hitPoint = raycastHit.point;
            //dest - origin
            var launchDirection = (hitPoint - throwDirection).normalized + Vector3.up * 0.25f;

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

    }
}
