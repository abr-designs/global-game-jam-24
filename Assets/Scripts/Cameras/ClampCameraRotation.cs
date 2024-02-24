using UnityEngine;
using Cinemachine;
 
[AddComponentMenu("")] // Hide in menu - use extensions menu
[SaveDuringPlay] [ExecuteAlways]
public class ClampCameraRotation : CinemachineExtension
{
    public float MinPitch = -20;
    public float MaxPitch = 20;
 
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Aim)
        {
            var rot = state.RawOrientation.eulerAngles;
            rot.x = Mathf.Clamp(rot.x, MinPitch, MaxPitch);
            state.RawOrientation = Quaternion.Euler(rot);
        }
    }
}