﻿using System;
using System.Collections;
using Cameras;
using GameInput;
using Levels;
using Prototype.Randall.Scripts.ScoringSystem;
using UI;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class GameplayController : MonoBehaviour
    {
        public static event Action OnLevelReady; 
        public static event Action<float, int> OnTimerCountdownUpdate; 
        public static event Action<string> OnDisplayText;
        //============================================================================================================//

        [SerializeField]
        private Vector3 startPosition;
        [SerializeField]
        private GameObject playerControllerPrefab;

        private GameObject _currentPlayerController;
        
        //============================================================================================================//
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void Start()
        {
            StartGame();
        }

        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= OnGameStateChanged;
        }
        
        //============================================================================================================//

        private void StartGame()
        {
            ScreenFader.ForceSetColorBlack();
            LevelLoader.LoadFirstLevel();
            StartCoroutine(LoadLevelCoroutine());
        }

        private void DisplayGameOptions(bool didWin)
        {
            if (didWin)
            {
                //TODO Need to return to menu here or something
                if (LevelLoader.OnLastLevel())
                {
                    GameplayUI.DisplayOptionWindow("You did it!", "To Menu", null);
                    return;
                }
                
                //TODO This text could be part of the level Controller data
                GameplayUI.DisplayOptionWindow("HA HA HA!", "Next Level", () =>
                {

                    LevelLoader.LoadNextLevel();
                    StartCoroutine(LoadLevelCoroutine());
                });
                return;
            }

            GameplayUI.DisplayOptionWindow("You think this is FUNNY?", "Try Again", () =>
            {

                LevelLoader.Restart();
                StartCoroutine(LoadLevelCoroutine());
            });
        }
        
        private static bool DidWin()
        {
            var scoreThreshold = LevelLoader.CurrentLevelController.minScoreToPass;
            var currentScore = FindObjectOfType<GameScoreManager>().GameScore;

            return currentScore >= scoreThreshold;
        }

        private void CreatePlayerController()
        {
            _currentPlayerController = Instantiate(playerControllerPrefab, startPosition, Quaternion.identity);
        }
        private void CleanPlayerController()
        {
            if (_currentPlayerController == null)
                return;
            
            Destroy(_currentPlayerController);
        }

        //GameLoop
        //============================================================================================================//
        
        private IEnumerator LoadLevelCoroutine()
        {
            //TODO Lock inputs
            //TODO Load a level
            //TODO Wait for level to be loaded
            //TODO Fade from black in cinematic view (Looking at King)
            //TODO Move Camera to gameplay position
            //TODO Unlock input
            //TODO Start timer
            //TODO Wait for timer to finish
            //TODO lock inputs

            CleanPlayerController();
            CreatePlayerController();
            OnLevelReady?.Invoke();
            GameInputDelegator.LockInputs = true;
            var gameplaySeconds = LevelLoader.CurrentLevelController.levelTime;
            
            CameraManager.SetCinematicCamera(CINEMATIC_CAMERA.THRONE, true);
            
            //Setup Timer
            OnTimerCountdownUpdate?.Invoke(1f, gameplaySeconds);
            
            yield return ScreenFader.FadeIn(1f, null);

            yield return new WaitForSeconds(1f);
            
            //TODO DO some intro cinematic
            
            CameraManager.SetCinematicCamera(CINEMATIC_CAMERA.DEFAULT);

            yield return new WaitForSeconds(CameraManager.CameraBlendTime);
            
            for (int i = 3; i > 0; i--)
            {
                OnDisplayText?.Invoke(i.ToString());
                yield return new WaitForSeconds(1f);
            }

            //------------------------------------------------//
            OnDisplayText?.Invoke("Go!");
            StartCoroutine(WaitCoroutine(1f, () =>
            {
                OnDisplayText?.Invoke(string.Empty);
            }));
            //------------------------------------------------//
            
            //TODO Maybe some countdown?
            GameInputDelegator.LockInputs = false;
            
            for (int i = 0; i < gameplaySeconds; i++)
            {
                OnTimerCountdownUpdate?.Invoke(1f - ((i + 1) /(float)gameplaySeconds), gameplaySeconds - i);
                yield return new WaitForSeconds(1f);
            }
            
            //Countdown finished
            OnTimerCountdownUpdate?.Invoke(0f, 0);
            OnDisplayText?.Invoke("Times Up!");
            
            GameInputDelegator.LockInputs = true;
            yield return new WaitForSeconds(1f);
            
            CameraManager.SetCinematicCamera(CINEMATIC_CAMERA.THRONE);
            
            OnDisplayText?.Invoke(string.Empty);
            yield return new WaitForSeconds(CameraManager.CameraBlendTime);
            
            //TODO Review Score
            //TODO Win or Lose
            var wonLevel = DidWin();
            
            //TODO Play some cinematic
            //TODO Fade to black

            ScreenFader.FadeOut(1f, () =>
            {
                DisplayGameOptions(wonLevel);
            });
        }

        private static IEnumerator WaitCoroutine(float seconds, Action onCompleted)
        {
            yield return new WaitForSeconds(seconds);
            onCompleted?.Invoke();
        }
        
        //============================================================================================================//

        private void OnGameStateChanged(GAME_STATE newGameState)
        {
            switch (newGameState)
            {
                case GAME_STATE.NONE:
                    return;
                case GAME_STATE.MENU:
                    //TODO Pause the game? Possibly caused by opening the settings menu
                    break;
                case GAME_STATE.GAME:
                    
                    break;
                case GAME_STATE.CINEMATIC:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
            }
        }
        //Editor Functions
        //============================================================================================================//

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(startPosition, 1f);
        }

        //============================================================================================================//
    }
}