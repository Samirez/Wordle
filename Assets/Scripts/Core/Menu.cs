using UnityEngine;
using Wordle.Board;

namespace Wordle.Core
{
    public class Menu : MonoBehaviour
    {
        private bool isMenuOpen = true;
        private GameObject menuUI, scoreUI, settingsUI, guessUI, gameOverUI;
        private BoardGenerator boardGenerator;

        public void Awake()
        {
            menuUI = GameObject.Find("MenuUI");

            if (menuUI == null)
            {
                menuUI = gameObject;
            }
            
            menuUI.SetActive(isMenuOpen);
            scoreUI = GameObject.Find("ScoreUI");

            if (scoreUI != null)
            {
                scoreUI.SetActive(false);
            }

            settingsUI = GameObject.Find("SettingsUI");

            if (settingsUI != null)
            {
                settingsUI.SetActive(false);
            }

            guessUI = GameObject.FindGameObjectsWithTag("GuessUI").Length > 0 ? GameObject.FindGameObjectsWithTag("GuessUI")[0] : null;
            
            if (guessUI != null)
            {
                guessUI.SetActive(false);
            }

            gameOverUI = GameObject.Find("GameOverUI");

            if (gameOverUI != null)            
            {
                gameOverUI.SetActive(false);
            }

            boardGenerator = FindFirstObjectByType<BoardGenerator>();

            if (boardGenerator == null)
            {
                Debug.LogWarning(
                    $"{name} ({GetType().Name}): BoardGenerator not found; board generation will be skipped.");
            }
        }

        public void StartGame()
        {
            if (boardGenerator == null)
            {
                Debug.LogError($"{name} ({GetType().Name}): Cannot start game; BoardGenerator not found.");
                return;
            }
            
            isMenuOpen = false;
            menuUI.SetActive(isMenuOpen);
            boardGenerator.InitializeBoard();
           
            if (guessUI != null)
            {
                guessUI.SetActive(true);
            }
        }
        public void HighScores()
        {
            isMenuOpen = false;
            menuUI.SetActive(isMenuOpen);
            if (scoreUI != null)
            {
                scoreUI.SetActive(true);
            }
        }

        public void OpenSettings()
        {
            isMenuOpen = false;
            menuUI.SetActive(isMenuOpen);
            if (settingsUI != null)
            {
                settingsUI.SetActive(true);
            }
        }

        public void ExitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void ShowGameOver()
        {
            if (guessUI != null)
            {
                guessUI.SetActive(false);
            }

            Destroy(boardGenerator.gameObject);

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
        }

        public void SubmitScore()
        {
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }

            isMenuOpen = true;
            menuUI.SetActive(isMenuOpen);
        }
    }
}