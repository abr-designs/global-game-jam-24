using Gameplay;
using UnityEngine;

namespace Utilities
{
    public class LevelLoadPositionReset : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        //Unity Functions
        //============================================================================================================//
        
        private void OnEnable()
        {
            GameplayController.OnLevelReady += OnLevelReady;
        }

        // Start is called before the first frame update
        private void Start()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void OnDisable()
        {
            GameplayController.OnLevelReady -= OnLevelReady;
        }
        //============================================================================================================//
    
        private void OnLevelReady()
        {
            transform.position = _startPosition;
            transform.rotation = _startRotation;
        }
    }
}
