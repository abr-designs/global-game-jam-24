using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoreManager : MonoBehaviour {

    public event EventHandler<ScorePointsArgs> OnPointsScored;

    public static GameScoreManager Instance { get; private set; }

    private int gameScore;

    private void Awake() {
        Instance = this;
    }

    public void ScorePoints(string description, int points, Vector3 worldPos) {

        gameScore += points;

        ScorePointsArgs args = new ScorePointsArgs();
        args.description = description;
        args.points = points;
        args.worldPos = worldPos;
        OnPointsScored?.Invoke(this, args);
    }

    public int GetGameScore() {
        return gameScore;
    }
}

public class ScorePointsArgs : EventArgs {
    public string description;
    public int points;
    public Vector3 worldPos;
}