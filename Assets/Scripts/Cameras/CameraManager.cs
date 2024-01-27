using System;
using Cinemachine;
using UnityEngine;
using Utilities;

namespace Cameras
{
    public enum CINEMATIC_CAMERA : int
    {
        DEFAULT, //Go to Default
        THRONE,
        
    }
    public class CameraManager : HiddenSingleton<CameraManager>
    {
        public static float CameraBlendTime => Instance.cameraBlendTime;
        [SerializeField, Min(0)]
        private float cameraBlendTime;
        
        [SerializeField]
        private Camera camera;
        [SerializeField]
        private CinemachineBrain brain;
        [SerializeField]
        private CinemachineSmoothPath dollyTrack;

        [SerializeField]
        private CinemachineVirtualCamera[] virtualCameras;

        //============================================================================================================//

        private void OnEnable()
        {
            CameraTarget.OnNewCameraTarget += SetDefaultCameraTarget;
        }

        // Start is called before the first frame update
        private void Start()
        {
            SetCamera(CINEMATIC_CAMERA.DEFAULT);
        }
        
        private void OnDisable()
        {
            CameraTarget.OnNewCameraTarget -= SetDefaultCameraTarget;
        }

        //============================================================================================================//
        
        private void SetCamera(CINEMATIC_CAMERA cinematicCamera)
        {
            var index = (int)cinematicCamera;
            for (var i = 0; i < virtualCameras.Length; i++)
            {
                virtualCameras[i].enabled = index == i;
                virtualCameras[i].Priority = index == i ? 1000 : -1000;
            }
        }

        private void ForceSetCamera(CINEMATIC_CAMERA cinematicCamera)
        {
            brain.enabled = false;
            var index = (int)cinematicCamera;
            for (var i = 0; i < virtualCameras.Length; i++)
            {
                virtualCameras[i].enabled = false;
            }
            var newCameraTransform = virtualCameras[index].transform;

            camera.transform.position = newCameraTransform.position;
            camera.transform.rotation = newCameraTransform.rotation;

            SetCamera(cinematicCamera);
            brain.enabled = true;
        }

        private void SetDefaultCameraTarget(Transform targetTransform)
        {
            var defaultCamera = virtualCameras[(int)CINEMATIC_CAMERA.DEFAULT];
            defaultCamera.Follow = targetTransform;
        }
        
        //============================================================================================================//

        public static void SetCinematicCamera(CINEMATIC_CAMERA cinematicCamera, bool forceSet = false)
        {
            if (forceSet)
            {
                Instance.ForceSetCamera(cinematicCamera);
                return;
            }
            
            Instance.SetCamera(cinematicCamera);
        }

    }
}
