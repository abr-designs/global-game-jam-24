using System;
using UnityEngine;
using Utilities.ReadOnly;

namespace Prototype.Randall.Scripts.ScoringSystem
{
    public class ScorePointsArgs : EventArgs
    {
        public string description;
        public int points;
        public Vector3 worldPos;
    }
    
    public class GameScoreManager : MonoBehaviour
    {
        public static event EventHandler<ScorePointsArgs> OnPointsScored;
        public static event Action<int> OnPointsTotalChanged;

        public int GameScore => gameScore;

        [SerializeField, ReadOnly] private int gameScore;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            PropObject.OnPointsScored += ScorePoints;
        }
    
        private void OnDisable()
        {
            PropObject.OnPointsScored -= ScorePoints;
        }
    
        //============================================================================================================//

        private void ScorePoints(string description, int points, Vector3 worldPos)
        {

            gameScore += points;
            OnPointsTotalChanged?.Invoke(gameScore);
            ScorePointsArgs args = new ScorePointsArgs
            {
                description = description,
                points = points,
                worldPos = worldPos,
            };
            OnPointsScored?.Invoke(this, args);

        }
    }
}

