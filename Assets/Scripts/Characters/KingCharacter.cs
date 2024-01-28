using System;
using Audio;
using Audio.SoundFX;
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

        private Animator Animator
        {
            get
            {
                if (_animator)
                    return _animator;

                _animator = GetComponent<Animator>();
                return _animator;
            }
            
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
                    Animator.Play("Idle");
                    break;
                case STATE.HAPPY:
                    Animator.Play("Laughing");

                    if (_laughingVFX == null)
                        _laughingVFX = VFX.LAUGH_BUBBLE.PlayAtLocation(vfxSpawnLocation.transform.position, keepAlive: true);

                    //TODO Need to figure out how to get this to loop!!
                    SFX.KING_HAPPY.PlaySound();
                    _laughingVFX.SetActive(true);
                    break;
                case STATE.ANGRY:
                    Animator.Play("Angry");
                    
                    
                    if (_angryVFX == null)
                        _angryVFX = VFX.ANGRY_BUBBLE.PlayAtLocation(vfxSpawnLocation.transform.position, keepAlive: true);

                    SFX.KING_ANGRY.PlaySound();
                    _angryVFX.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
    }
}
