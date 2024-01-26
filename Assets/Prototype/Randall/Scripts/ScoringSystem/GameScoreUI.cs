using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameScoreText;

    private void Start() {

        GameScoreManager.Instance.OnPointsScored += GameScoreManager_OnPointsScored;

    }

    private void GameScoreManager_OnPointsScored(object sender, System.EventArgs e) {

        UpdateVisual();

    }

    private void UpdateVisual() {

        gameScoreText.text = "Score: " + GameScoreManager.Instance.GetGameScore();

    }
}

