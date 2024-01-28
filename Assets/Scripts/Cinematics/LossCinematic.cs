using System.Collections;
using Cameras;
using Characters;
using Levels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinematics
{
    public class LossCinematic : CinematicBase
    {
        // Start is called before the first frame update
        public override bool IsPlaying { get; protected set; }
        protected override bool KeepCamera => true;

        [SerializeField, Min(0f)] private float startDelay;

        [SerializeField, Min(0), Header("FOV")]
        private float fovTarget;

        [SerializeField, Min(0)] private float betweenDelay;

        [SerializeField] private AnimationCurve fovInCurve;
        [SerializeField, Min(0f)] private float fovInTime;

        [SerializeField] private float targetFOV2;
        [SerializeField] private AnimationCurve fovOutCurve;
        [SerializeField, Min(0f)] private float zoomTime;


        [SerializeField, Header("Dutch Angle")]
        private float dutchAngle;
        [SerializeField] private AnimationCurve dutchCurve;

        private float _startFov;

        [SerializeField, Header("Chain Cinematics")]
        private CinematicBase[] playNextCinematics;

        //------------------------------------------------//

//============================================================================================================//
        private void Start()
        {
            targetCamera.LookAt = _kingCharacter.transform;
        }

        //============================================================================================================//

        protected override IEnumerator PlayCinematicCoroutine()
        {
            IsPlaying = true;
            CameraManager.SetDefaultCameraTargets(null);

            yield return new WaitForSeconds(startDelay);

            _startFov = targetCamera.m_Lens.FieldOfView;

            for (float t = 0; t < fovInTime; t += Time.deltaTime)
            {
                var td = t / fovInTime;

                targetCamera.m_Lens.FieldOfView = Mathf.Lerp(_startFov, fovTarget, fovInCurve.Evaluate(td));
                yield return null;
            }

            _kingCharacter.SetState(KingCharacter.STATE.ANGRY);

            yield return new WaitForSeconds(betweenDelay);

            for (float t = 0; t < zoomTime; t += Time.deltaTime)
            {
                var td = t / zoomTime;

                targetCamera.m_Lens.FieldOfView = Mathf.Lerp(fovTarget, targetFOV2, fovOutCurve.Evaluate(td));
                targetCamera.m_Lens.Dutch = Mathf.Lerp(0f, dutchAngle, dutchCurve.Evaluate(td));
                yield return null;
            }

            //Wait for the player to press the continue button
            //------------------------------------------------//

            targetCamera.transform.SetParent(LevelLoader.CurrentLevelController.transform, true);
            
            //TODO Pick random other animation
            var deathCinematicInstance = Instantiate(playNextCinematics[Random.Range(0, playNextCinematics.Length)], transform);
            

            yield return deathCinematicInstance.StartCinematic();
        }
    }
}