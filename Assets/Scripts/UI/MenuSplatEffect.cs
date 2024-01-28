using System.Collections;
using System.Collections.Generic;
using Audio;
using Audio.SoundFX;
using UnityEngine;
using UnityEngine.UI;

public class MenuSplatEffect : MonoBehaviour
{
    [SerializeField]
    private RawImage image;

    private RectTransform imageRect;

    [SerializeField, Min(0)]
    private float minSize;
    [SerializeField, Min(0)]
    private float maxSize;

    [SerializeField, Min(0)]
    private float fadeTime;

    [SerializeField]
    private Color32[] colors;
    
    private Vector2 minPosition;
    private Vector2 maxPosition;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        imageRect = image.transform as RectTransform;
        InitPositionRange();

        StartCoroutine(SplatEffectCoroutine());
    }

    private void InitPositionRange()
    {
        var halfScreenWidth = Screen.width / 2f;
        var halfScreenHeight = Screen.height / 2f;

        minPosition = new Vector2(-halfScreenWidth, -halfScreenHeight);
        maxPosition = new Vector2(halfScreenWidth, halfScreenHeight);
    }

    IEnumerator SplatEffectCoroutine()
    {
        image.color = Color.clear;
        
        while (true)
        {
            yield return new WaitForSeconds(GetRandomWaitTime());

            var position = new Vector2(Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y));
            var rotation = Random.Range(0, 360);
            var size = Vector2.one * Random.Range(minSize, maxSize);

            var color = colors[Random.Range(0, colors.Length)];
            var clearColor = color;
            clearColor.a = 0;

            SFX.SQUELCH.PlaySound(0.2f);
            
            var fadeTime = GetRandomFadeTime();

            imageRect.localPosition = position;
            imageRect.rotation = Quaternion.Euler(0f, rotation, 0f);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            image.color = color;
            
            for (float t = 0; t < fadeTime; t+=Time.deltaTime)
            {
                image.color = Color32.Lerp(color, clearColor, t / fadeTime);
                yield return null;
            }

        }
    }

    private float GetRandomWaitTime()
    {
        return Random.Range(1f, 10f);
    }
    
    private float GetRandomFadeTime()
    {
        return Random.Range(fadeTime / 2.5f, fadeTime);
    }

}
