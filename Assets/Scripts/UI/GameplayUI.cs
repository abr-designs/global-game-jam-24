using System;
using Gameplay;
using Prototype.Randall.Scripts.ScoringSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class GameplayUI : HiddenSingleton<GameplayUI>
    {
        [SerializeField]
        private TMP_Text textDisplay;
        
        [SerializeField, Header("Timer")]
        private TMP_Text timerText;
        [SerializeField]
        private Slider timerSlider;
        
        [SerializeField, Header("Scoring")] 
        private TMP_Text gameScoreText;

        [SerializeField, Header("Option Window")]
        private GameObject optionWindow;
        [SerializeField]
        private TMP_Text optionText;
        [SerializeField]
        private Button optionButton;
        [SerializeField]
        private TMP_Text optionButtonText;

        //============================================================================================================//

        private void OnEnable()
        {
            GameScoreManager.OnPointsTotalChanged += OnPointsTotalChanged;
            GameplayController.OnTimerCountdownUpdate += OnTimerCountdownUpdate;
            GameplayController.OnDisplayText += OnDisplayText;
        }

        // Start is called before the first frame update
        private void Start()
        {
            InitUI();
        }
        
        private void OnDisable()
        {
            GameScoreManager.OnPointsTotalChanged -= OnPointsTotalChanged;
            GameplayController.OnTimerCountdownUpdate -= OnTimerCountdownUpdate;
            GameplayController.OnDisplayText -= OnDisplayText;
        }
        
        //============================================================================================================//

        private void InitUI()
        {
            textDisplay.text = string.Empty;
            timerText.text = string.Empty;
            timerSlider.value = 1f;
            
            OnPointsTotalChanged(0);
            optionWindow.SetActive(false);
        }
        //============================================================================================================//

        public static void DisplayOptionWindow(string displayText, string optionText, Action onButtonPressed)
        {
            Instance?.TryDisplayOptionWindow(displayText,optionText, onButtonPressed);
        }

        private void TryDisplayOptionWindow(string displayText, string buttonText, Action onButtonPressed)
        {
            optionWindow.SetActive(true);
            optionText.text = displayText;
            optionButtonText.text = buttonText;
            optionButton.onClick.RemoveAllListeners();
            optionButton.onClick.AddListener(() =>
            {
                onButtonPressed?.Invoke();
                optionWindow.SetActive(false);
            });
        }
        
        //============================================================================================================//

        private void OnTimerCountdownUpdate(float timerValue, int seconds)
        {
            timerText.text = $"{seconds}s";
            timerSlider.value = timerValue;
        }
        
        private void OnDisplayText(string text)
        {
            textDisplay.text = text;
        }
        
        private void OnPointsTotalChanged(int pointsTotal)
        {
            gameScoreText.text = $"Score: {pointsTotal}";
        }
        
        //============================================================================================================//

    }
}
