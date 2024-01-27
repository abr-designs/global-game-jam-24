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

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {

        popupCountdown -= Time.deltaTime;

        rectTransform.anchoredPosition += Vector2.up * Time.deltaTime * floatSpeed;

        if(popupCountdown < 0) {
            Destroy(gameObject);
        }

    }

    public void InitializeVisual(ScorePointsArgs args) {

        pointsDescriptionText.text = args.description;
        pointsAmountText.text = args.points.ToString();

        popupCountdown = popupDuration;

        if (placeInWorldPos) {
            Vector3 targetPosition = Vector3.zero;
            targetPosition = args.worldPos;

            // Convert the target object's world position to screen space
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.position);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPosition);

            // Set the UI element's position to the screen position
            //RectTransform rt = GetComponent<RectTransform>();

            rectTransform.position = screenPos;

            rectTransform.anchoredPosition += new Vector2(0, worldPosOffset);
        }

        if(scalePopup) {
            rectTransform.localScale = Vector3.one * worldPosScaleFactor;
        }
    }
}
