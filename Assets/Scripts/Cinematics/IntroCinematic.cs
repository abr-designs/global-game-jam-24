using System.Collections;
using Levels;
using UnityEngine;

namespace Cinematics
{
    public class IntroCinematic : CinematicBase
    {
        public override bool IsPlaying { get; protected set; }

        [SerializeField, Min(0)]
        private float delay;

        [SerializeField]
        private Transform bubbleTransform;

        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        
        protected override IEnumerator PlayCinematicCoroutine()
        {
            bubbleTransform.gameObject.SetActive(false);
            bubbleTransform.forward = (bubbleTransform.position - Camera.main.transform.position).normalized;
            
            IsPlaying = true;
            ForceSetCameraPosition(targetCamera);

            var avoidObjects = LevelLoader.CurrentLevelController.avoidObjects;

            if (avoidObjects.Length >= 0)
            {
                bubbleTransform.gameObject.SetActive(true);
                foreach (var objectSo in avoidObjects)
                {
                    spriteRenderer.sprite = objectSo.icon;

                    yield return new WaitForSeconds(1f);
                }
                bubbleTransform.gameObject.SetActive(false);
            }

            //TODO Get all items to avoid & tell the player
            yield return new WaitForSeconds(delay);
            
            IsPlaying = false;
        }
    }
}