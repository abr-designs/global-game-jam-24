using System;
using System.Collections;
using Audio;
using Audio.SoundFX;
using Unity.VisualScripting;
using UnityEngine;
using VisualFX;

namespace Characters
{
    public class GuardCharacter : MonoBehaviour
    {
        [SerializeField]
        private float RepelForce = 200f;

        [SerializeField]
        private Transform HeadVFXPosition;

        // Flag to prevent multiple triggers firing in the same window
        private bool _isTriggered = false;

        private void OnCollisionEnter(Collision collision)
        {
            if(!_isTriggered && collision.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player hit guard {collision.collider.name}");
                var player = collision.collider.GetComponentInParent<WASDRagdollController>();
                player.StunPlayer(3f);

                Vector3 groundDir = Vector3.ProjectOnPlane(collision.transform.position - transform.position, Vector3.up).normalized;
                Vector3 dir = Vector3.Normalize(groundDir + Vector3.up * .25f);
                player.ApplyForce(dir, RepelForce);
                //collision.rigidbody.AddForce(dir.normalized * RepelForce, ForceMode.Force);

                //var push = VFX.PUSH.PlayAtLocation(collision.contacts[0].point);
                //push.transform.rotation = Quaternion.LookRotation(groundDir);
                var alert = VFX.GUARD_ALERT.PlayAtLocation(HeadVFXPosition.position);
                alert.transform.parent = HeadVFXPosition;
                alert.transform.rotation = HeadVFXPosition.rotation;

                StartCoroutine(TriggerCoroutine(.5f));

            }
        }

        private IEnumerator TriggerCoroutine(float waitTime)
        {
            _isTriggered = true;
            yield return new WaitForSeconds(waitTime);
            _isTriggered = false;
        }
    }
}
