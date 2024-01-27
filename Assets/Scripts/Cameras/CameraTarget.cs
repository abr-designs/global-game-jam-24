using System;
using UnityEngine;

namespace Cameras
{
    public class CameraTarget : MonoBehaviour
    {
        public static event Action<Transform> OnNewCameraTarget;


        private void Start()
        {
            OnNewCameraTarget?.Invoke(transform);
        }
    }
}
