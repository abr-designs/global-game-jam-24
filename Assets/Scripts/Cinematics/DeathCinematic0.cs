using System.Collections;
using Cameras;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cinematics
{
    public class DeathCinematic0 : CinematicBase
    {
        // Start is called before the first frame update
        public override bool IsPlaying { get; protected set; }

        [SerializeField]
        private Transform playerMoveToPosition;
        [SerializeField]
        private Rigidbody[] crusherObjects;

        [SerializeField]
        private AnimationCurve fallCurve;

        //Unity Functions
        //============================================================================================================//
        
        private void Start()
        {
            for (int i = 0; i < crusherObjects.Length; i++)
            {
                crusherObjects[i].gameObject.SetActive(false);
                crusherObjects[i].isKinematic = true;
            }

        }
        
        //============================================================================================================//

        protected override IEnumerator PlayCinematicCoroutine()
        {
            if (_playerKinematicRoot == null)
                FindObjectOfType<HandsController>().transform
                    .FindObjectWithName("KinematicRoot", out _playerKinematicRoot);
            
            var playerPuppet = FindObjectOfType<PuppetRagdoll>();

            IsPlaying = true;
            targetCamera.Priority = 100000;

            _playerKinematicRoot.position = playerMoveToPosition.position;
            _playerKinematicRoot.rotation = playerMoveToPosition.rotation;
            
            yield return new WaitForSeconds(CameraManager.CameraBlendTime);

            var selectedCrusher = crusherObjects[Random.Range(0, crusherObjects.Length)];
            selectedCrusher.transform.position = playerMoveToPosition.position + Vector3.up * 10f;

            yield return new WaitForSeconds(Random.Range(0.5f, 3f));

            selectedCrusher.gameObject.SetActive(true);

            var startPosition = selectedCrusher.transform.position;
            var endPosition = playerMoveToPosition.position;

            for (float t = 0; t < 1f; t+=Time.deltaTime)
            {
                selectedCrusher.transform.position = Vector3.Lerp(startPosition, endPosition, fallCurve.Evaluate(t / 1f));
                yield return null;
            }

            playerPuppet.EnableRagdoll(true);
            yield return new WaitForSeconds(Random.Range(0.5f, 3f));
            
            IsPlaying = false;
        }
    }
}
