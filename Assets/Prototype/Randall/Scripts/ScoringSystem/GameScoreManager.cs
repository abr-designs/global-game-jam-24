using System;
using Gameplay;
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
            GameplayController.OnLevelReady += OnLevelReady;
        }
        
        private void OnDisable()
        {
            PropObject.OnPointsScored -= ScorePoints;
            GameplayController.OnLevelReady -= OnLevelReady;
        }
    
        //============================================================================================================//

        private void ScorePoints(string description, int points, Vector3 worldPos)
        {
            //Clamp the score above 0
            var newScore = gameScore + points;
            if (newScore < 0)
            {
                newScore = 0;
            }

            //See if the score changed
            if (newScore != gameScore)
            {
                gameScore = newScore;
                OnPointsTotalChanged?.Invoke(gameScore);
            }


            ScorePointsArgs args = new ScorePointsArgs
            {
                description = description,
                points = points,
                worldPos = worldPos,
            };
            OnPointsScored?.Invoke(this, args);

        }
        
        private void OnLevelReady()
        {
            if (gameScore == 0)
                return;
            
            gameScore = 0;
            OnPointsTotalChanged?.Invoke(gameScore);
        }
    }
}

