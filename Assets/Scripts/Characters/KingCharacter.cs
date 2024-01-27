using System;
using UnityEngine;
using VisualFX;

namespace Characters
{
    [RequireComponent(typeof(Animator))]
    public class KingCharacter : MonoBehaviour
    {
        public enum STATE
        {
            DEFAULT,
            HAPPY,
            ANGRY
        }

        private Animator _animator;

        [SerializeField] private STATE currentState;

        [SerializeField]
        private Transform vfxSpawnLocation;

        private GameObject _laughingVFX;
        private GameObject _angryVFX;
        
        // Start is called before the first frame update
        private void Start()
        {
            _animator = GetComponent<Animator>();
            SetState(STATE.DEFAULT);
        }

        public void SetState(STATE state)
        {
            _angryVFX?.SetActive(false);
            _laughingVFX?.SetActive(false);
            currentState = state;
            
            switch (state)
            {
                case STATE.DEFAULT:
                    _animator.Play("Idle");
                    break;
                case STATE.HAPPY:
                    _animator.Play("Laughing");

                    if (_laughingVFX == null)
                        _laughingVFX = VFX.LAUGH_BUBBLE.PlayAtLocation(vfxSpawnLocation.transform.position, keepAlive: true);

                    _laughingVFX.SetActive(true);
                    break;
                case STATE.ANGRY:
                    _animator.Play("Angry");
                    
                    
                    if (_angryVFX == null)
                        _angryVFX = VFX.ANGRY_BUBBLE.PlayAtLocation(vfxSpawnLocation.transform.position, keepAlive: true);

                    _angryVFX.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
    }
}
