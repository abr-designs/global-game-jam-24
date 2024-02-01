using Unity.VisualScripting;
using UnityEngine;

namespace Prototype.Alex.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField]
        private Vector3 localPivotOffset;
        [SerializeField]
        private LayerMask groundMask;
        private FixedJoint _joint;
        private Rigidbody _rigidbody;
        private float _initialMass;

        private Rigidbody _target;

        //Unity Functions
        //============================================================================================================//

        // Start is called before the first frame update
        private void Start()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            _initialMass = _rigidbody.mass;
        }

        //============================================================================================================//

        private void Update() 
        {
            if(_target)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _target.transform.rotation, 5f);
                transform.position = Vector3.MoveTowards(transform.position, _target.position, .05f);
            }
        }

        public void Pickup(Vector3 worldPosition, Rigidbody attachTo)
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

            // OLD CODE
            /*
            _rigidbody.mass = _initialMass;
            _joint.connectedBody = null;
            Destroy(_joint);
            */
        }

        public void Throw(Vector3 throwDirection, float launchForce)
        {

            _rigidbody.isKinematic = false;
            _rigidbody.detectCollisions = true;
            _target = null;

            var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenPointToRay, out var raycastHit, 100, groundMask.value) == false)
                return;

            var hitPoint = raycastHit.point;
            //dest - origin
            var launchDirection = (hitPoint - throwDirection).normalized + Vector3.up * 0.25f;

            _rigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            Debug.DrawLine(throwDirection, hitPoint, Color.cyan, 5.0f, true);

            // OLD CODE BELOW
            /*
            _rigidbody.mass = _initialMass;
            _joint.connectedBody = null;
            Destroy(_joint);

            var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(screenPointToRay, out var raycastHit, 100, groundMask.value) == false)
                return;

            var hitPoint = raycastHit.point;
            //dest - origin
            var launchDirection = (hitPoint - throwDirection).normalized + Vector3.up * 0.25f;

            _rigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            Debug.DrawLine(throwDirection, hitPoint, Color.cyan, 5.0f, true);
            */
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
    }
}
