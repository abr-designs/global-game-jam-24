using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
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
