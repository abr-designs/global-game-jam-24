using System.Collections;
using UnityEngine;

namespace Cinematics
{
    public class IntroCinematic : CinematicBase
    {
        public override bool IsPlaying { get; protected set; }

        [SerializeField, Min(0)]
        private float delay;
        
        protected override IEnumerator PlayCinematicCoroutine()
        {
            IsPlaying = true;
            ForceSetCameraPosition(targetCamera);
            
            //TODO Get all items to avoid & tell the player

            yield return new WaitForSeconds(delay);


            
            IsPlaying = false;
        }
    }
}