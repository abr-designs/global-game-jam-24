using System.Collections;
using Cameras;
using Characters;
using Gameplay;
using Levels;
using UnityEngine;

namespace Cinematics
{
    public class VictoryCinematic : CinematicBase
    {
        // Start is called before the first frame update
        public override bool IsPlaying { get; protected set; }
        protected override bool KeepCamera => true;

        [SerializeField, Min(0f)]
        private float startDelay;

        [SerializeField, Min(0), Header("FOV")]
        private float fovTarget;

        [SerializeField, Min(0)]
        private float betweenDelay;

        [SerializeField]
        private AnimationCurve fovInCurve;
        [SerializeField, Min(0f)]
        private float fovInTime;
    
        [SerializeField]
        private AnimationCurve fovOutCurve;
        [SerializeField, Min(0f)]
        private float fovOutTime;

        private float _startFov;

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
        
            _kingCharacter.SetState(KingCharacter.STATE.HAPPY);

            yield return new WaitForSeconds(betweenDelay);
        
            for (float t = 0; t < fovOutTime; t += Time.deltaTime)
            {
                var td = t / fovOutTime;

                targetCamera.m_Lens.FieldOfView = Mathf.Lerp(fovTarget, _startFov, fovOutCurve.Evaluate(td));
                yield return null;
            }

            //Wait for the player to press the continue button
            //------------------------------------------------//
            
            targetCamera.transform.SetParent(LevelLoader.CurrentLevelController.transform, true);
        }

    }
}
