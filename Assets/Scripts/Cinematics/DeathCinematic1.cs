using System.Collections;
using Cameras;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cinematics
{
    public class DeathCinematic1 : CinematicBase
    {
        // Start is called before the first frame update
        public override bool IsPlaying { get; protected set; }

        [SerializeField] private Transform trainTransform;

        [SerializeField] private float trainMoveTime;
        [SerializeField] private Transform playerMoveToPosition;
        [SerializeField] private Transform startPosition;
        [SerializeField] private Transform endPosition;

        [SerializeField]
        private float doorScaleTime;
        [SerializeField]
        private AnimationCurve doorScaleCurve;

        [SerializeField] 
        private Transform[] doors;

        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].localScale = Vector3.zero;
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

            for (float t = 0; t < doorScaleTime; t+=Time.deltaTime)
            {
                var td = t / doorScaleTime;

                for (int ii = 0; ii < doors.Length; ii++)
                {
                    doors[ii].localScale = Vector3.one * Mathf.Lerp(0f, 1f, doorScaleCurve.Evaluate(td));
                }
                
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 3f));

            playerPuppet.EnableRagdoll(true);
            for (float t = 0; t < trainMoveTime; t += Time.deltaTime)
            {
                trainTransform.position = Vector3.Lerp(startPosition.position, endPosition.position, t / trainMoveTime);
                yield return null;
            }
            
            yield return new WaitForSeconds(Random.Range(0.5f, 3f));
            
            for (float t = 0; t < doorScaleTime; t+=Time.deltaTime)
            {
                var td = t / doorScaleTime;

                for (int ii = 0; ii < doors.Length; ii++)
                {
                    doors[ii].localScale = Vector3.one * Mathf.Lerp(1f, 0f, doorScaleCurve.Evaluate(td));
                }
                
                yield return null;
            }

            IsPlaying = false;
        }
    }
}
