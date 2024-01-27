using System;
using System.Collections;
using Cameras;
using GameInput;
using Levels;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class GameplayController : MonoBehaviour
    {
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
            StartCoroutine(LoadLevelCoroutine(LoadNextLevel));
        }

        private IEnumerator LoadLevelCoroutine(Action onLevelFinished)
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
            
            GameInputDelegator.LockInputs = true;
            
            CameraManager.SetCinematicCamera(CINEMATIC_CAMERA.THRONE, true);

            yield return ScreenFader.FadeIn(1f, null);

            yield return new WaitForSeconds(1f);
            
            //TODO DO some intro cinematic
            
            CameraManager.SetCinematicCamera(CINEMATIC_CAMERA.DEFAULT);

            yield return new WaitForSeconds(CameraManager.CameraBlendTime);
            
            //TODO Maybe some countdown?
            
            GameInputDelegator.LockInputs = false;

            var gameplaySeconds = LevelLoader.CurrentLevelController.levelTime;
            for (int i = 0; i < gameplaySeconds; i++)
            {
                yield return new WaitForSeconds(1f);
            }
            
            GameInputDelegator.LockInputs = true;
            yield return new WaitForSeconds(1f);
            
            CameraManager.SetCinematicCamera(CINEMATIC_CAMERA.THRONE);
            //TODO Review Score
            //TODO Win or Lose
            //TODO Play some cinematic
            //TODO Fade to black

            //TODO Restart of Move to next
            ScreenFader.FadeOut(1f, () =>
            {
                onLevelFinished?.Invoke();
            });
        }

        private void LoadNextLevel()
        {
            if (LevelLoader.LoadNextLevel() == false)
            {
                Debug.LogError("Game Over");
                return;
            }

            StartCoroutine(LoadLevelCoroutine(LoadNextLevel));
        }


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
    }
}