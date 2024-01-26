using Cinemachine;
using UnityEngine;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Camera camera;
        [SerializeField]
        private CinemachineVirtualCamera defaultVirtualCamera;
        [SerializeField]
        private CinemachineSmoothPath dollyTrack;
        [SerializeField]
        private CinemachineVirtualCamera throneVirtualCamera;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

    }
}
