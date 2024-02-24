using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.Randall.Scripts.ScoringSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PointsPopup : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI pointsDescriptionText;
    [SerializeField] private TextMeshProUGUI pointsAmountText;

    private RectTransform rectTransform;

    [SerializeField] private float popupDuration = 1.5f;
    private float popupCountdown;

    [SerializeField] private float floatSpeed = 30f;
    [SerializeField] private bool placeInWorldPos = true;
    [SerializeField] private float worldPosOffset = -200f;
    [SerializeField] private bool scalePopup = true;
    [SerializeField] private float worldPosScaleFactor = 0.5f;

    [SerializeField] private Vector3 pointsFlyToLocation;
    private float _pointsFlyCounter = 0f;
    private Vector3 _pointsFlyStart = Vector3.zero;
    private Vector3 _pointsFlyStartScale = Vector3.zero;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        if(popupCountdown > 0)
        {
            popupCountdown -= Time.deltaTime;
            rectTransform.anchoredPosition += Vector2.up * Time.deltaTime * floatSpeed;
            return;
        } else
        {
            _pointsFlyCounter += Time.deltaTime;
            
            if(_pointsFlyStart == Vector3.zero)
                _pointsFlyStart = rectTransform.position;
            if(_pointsFlyStartScale == Vector3.zero)
                _pointsFlyStartScale = rectTransform.localScale;

            rectTransform.position = Vector3.Lerp(_pointsFlyStart, pointsFlyToLocation, _pointsFlyCounter); 
            rectTransform.localScale = Vector3.Lerp(_pointsFlyStartScale, Vector3.one * .1f, _pointsFlyCounter);
            
            if(_pointsFlyCounter > 1f)
            {
                Destroy(gameObject);
            }
        }         
            

    }

    public void InitializeVisual(ScorePointsArgs args) {

        pointsDescriptionText.text = args.description;
        pointsAmountText.text = args.points.ToString();

        if(args.points < 0)
        {
            pointsDescriptionText.color = Color.red;
            pointsAmountText.color = Color.red;
        }

        popupCountdown = popupDuration;

        if (placeInWorldPos) {
            Vector3 targetPosition = Vector3.zero;
            targetPosition = args.worldPos;

            // Convert the target object's world position to screen space
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.position);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPosition);
            Debug.Log($"Screenpos popup - {targetPosition} {screenPos}");

            // Set the UI element's position to the screen position
            //RectTransform rt = GetComponent<RectTransform>();

            // Clamp the popup to the screen bounds
            screenPos.x = Mathf.Clamp(screenPos.x, 0, Camera.main.pixelWidth);
            screenPos.y = Mathf.Clamp(screenPos.y, 0, Camera.main.pixelHeight * .85f);
    
            rectTransform.position = screenPos;

            rectTransform.anchoredPosition += new Vector2(0, worldPosOffset);
        }

        if(scalePopup) {
            rectTransform.localScale = Vector3.one * worldPosScaleFactor;
        }
    }
}
