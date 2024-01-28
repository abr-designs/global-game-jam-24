using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(Rigidbody))]
    public class ForceNonKinematic : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.rigidbody == null)
                return;
            if (other.rigidbody.isKinematic == false)
                return;
            
            other.rigidbody.isKinematic = false;
        }
    }
}
