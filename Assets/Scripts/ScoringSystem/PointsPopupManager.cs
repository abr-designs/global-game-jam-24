using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsPopupManager : MonoBehaviour {

    [SerializeField] private GameObject pointsPopupPrefab;

    [SerializeField] private bool clearExistingPopups = true;

    private void Start() {

        GameScoreManager.Instance.OnPointsScored += GameScoreManager_OnPointsScored;

    }

    private void GameScoreManager_OnPointsScored(object sender, ScorePointsArgs args) {

        if(clearExistingPopups) {
            foreach(Transform child in transform) {
                Destroy(child.gameObject);
            }
        }

        GeneratePopup(args);

    }

    private void GeneratePopup(ScorePointsArgs args) {
        
        GameObject newPopup = Instantiate(pointsPopupPrefab, transform);
        PointsPopup pointsPopup = newPopup.GetComponent<PointsPopup>();

        pointsPopup.PopulateVisual(args.description, args.points);
    }
}
