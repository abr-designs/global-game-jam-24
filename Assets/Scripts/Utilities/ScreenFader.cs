using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class ScreenFader : HiddenSingleton<ScreenFader>
    {
        private static readonly Color32 black = new Color32(0, 0, 0, 1);
        private static readonly Color32 clear = new Color32(0, 0, 0, 0);
        
        [SerializeField]
        private Image blackImage;

        //
        //============================================================================================================//

        public static void ForceSetColorBlack()
        {
            Instance.blackImage.color = black;
        }
        public static void ForceSetColorClear()
        {
            Instance.blackImage.color = clear;
        }

        public static Coroutine FadeInOut(float time, Action onFaded, Action onComplete)
        {
            return Instance.StartCoroutine(Instance.FadeInOutCoroutine(time, onFaded, onComplete));
        }
        
        public static Coroutine FadeOut(float time, Action onComplete)
        {
            return Instance.StartCoroutine(Instance.FadeCoroutine(clear, black, time, onComplete));
        }
        
        public static Coroutine FadeIn(float time, Action onComplete)
        {
            return Instance.StartCoroutine(Instance.FadeCoroutine(black, clear, time, onComplete));
        }
        
        //Instance Coroutines
        //============================================================================================================//

        private IEnumerator FadeInOutCoroutine(float time, Action onFaded, Action onComplete)
        {
            var halfTime = time / 2f;

            yield return StartCoroutine(FadeCoroutine(clear, black, halfTime, onFaded));
            
            yield return StartCoroutine(FadeCoroutine(black, clear, halfTime, onComplete));
        }

        private IEnumerator FadeCoroutine(Color32 startColor, Color32 endColor, float time, Action onCompleted)
        {
            blackImage.color = startColor;

            for (float t = 0; t < time; t+= Time.deltaTime)
            {
                blackImage.color = Color32.Lerp(startColor, endColor, t / time);
                yield return null;
            }
            
            blackImage.color = endColor;
            onCompleted?.Invoke();
        }
        //============================================================================================================//
    }
}