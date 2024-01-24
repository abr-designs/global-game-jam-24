using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsPopup : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI pointsDescriptionText;
    [SerializeField] private TextMeshProUGUI pointsAmountText;

    private float popupDuration = 2f;
    private float popupCountdown;

    private void Update() {

        popupCountdown -= Time.deltaTime;

        if(popupCountdown < 0) {
            Destroy(gameObject);
        }

    }

    public void PopulateVisual(string description, int points) {

        pointsDescriptionText.text = description;
        pointsAmountText.text = points.ToString();

        popupCountdown = popupDuration;
    }
}
