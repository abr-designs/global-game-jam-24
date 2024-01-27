using System;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class GameStateManager : HiddenSingleton<GameStateManager>
    {
        public static event Action<GAME_STATE> OnGameStateChanged; 
        //============================================================================================================//

        private GAME_STATE _currentGameState;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        //============================================================================================================//

        public static void SetGameState(GAME_STATE gameState)
        {
            if (gameState == Instance._currentGameState)
                return;

            Instance._currentGameState = gameState;
            OnGameStateChanged?.Invoke(gameState);
        }
    }
}
