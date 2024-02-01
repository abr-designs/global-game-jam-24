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

        //Unity Functions
        //============================================================================================================//

        // Start is called before the first frame update
        private void Start()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            _initialMass = _rigidbody.mass;
        }

        //============================================================================================================//

        public void Pickup(Vector3 worldPosition, Rigidbody attachTo)
        {
            if (_rigidbody.isKinematic == true)
            {
                _rigidbody.isKinematic = false;
            }
            _rigidbody.mass = 0;

            transform.position = worldPosition + localPivotOffset;
            transform.localRotation = attachTo.rotation;
            _joint = gameObject.AddComponent<FixedJoint>();
            _joint.connectedBody = attachTo;
        }

        public void Drop()
        {
            _rigidbody.mass = _initialMass;
            _joint.connectedBody = null;
            Destroy(_joint);
        }

        public void Throw(Vector3 throwDirection, float launchForce)
        {
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
