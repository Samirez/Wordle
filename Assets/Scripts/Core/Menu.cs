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

            var guessObjects = GameObject.FindGameObjectsWithTag("GuessUI");
            guessUI = guessObjects.Length > 0 ? guessObjects[0] : null;
            
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
            if (!boardGenerator.gameObject.activeSelf)
            {
                boardGenerator.gameObject.SetActive(true);
            }
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

            if (boardGenerator != null)
            {
                boardGenerator.gameObject.SetActive(false);
            }

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
                
            }
        }

        public void ReturnToMenu()
        {
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }

            isMenuOpen = true;
            menuUI.SetActive(isMenuOpen);
            if (boardGenerator != null && !boardGenerator.gameObject.activeSelf)
            {
                boardGenerator.gameObject.SetActive(true);
            }
        }
    }
}