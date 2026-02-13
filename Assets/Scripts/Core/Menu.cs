using UnityEngine;
using TMPro;
using Wordle.Board;
using Wordle.HighScoreStorage;
using UnityEngine.UI;

namespace Wordle.Core
{
    public class Menu : MonoBehaviour
    {
        private bool isMenuOpen = true;
        private GameObject menuUI, scoreUI, settingsUI, guessUI, gameOverUI;
        private BoardGenerator boardGenerator;
        private PlayerRecords playerRecords;
        private AudioSource soundtrack;
        public void Awake()
        {
            menuUI = GameObject.Find("MenuUI");

            if (menuUI == null)
            {
                menuUI = gameObject;
            }
            
            menuUI.SetActive(isMenuOpen);
            scoreUI = GameObject.FindGameObjectWithTag("ScoreUI");

            if (scoreUI != null)
            {
                scoreUI.SetActive(false);
            }

            settingsUI = GameObject.Find("SettingsUI");

            if (settingsUI != null)
            {
                settingsUI.SetActive(false);
            }

            guessUI = GameObject.FindGameObjectWithTag("GuessUI");
            
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
            soundtrack = GetComponent<AudioSource>();
            if (soundtrack == null)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): AudioSource for soundtrack not found.");
            }
        }

        void Start()
        {
            playerRecords = FindFirstObjectByType<PlayerRecords>();

            if (playerRecords == null)
            {
                Debug.LogWarning(
                    $"{name} ({GetType().Name}): PlayerRecords not found; high score storage will be unavailable.");
            }
        }

        public void StartGame()
        {
            if (soundtrack != null && !soundtrack.isPlaying)
            {
                soundtrack.Play();
            }

            if (boardGenerator == null)
            {
                Debug.LogError($"{name} ({GetType().Name}): Cannot start game; BoardGenerator not found.");
                return;
            }
            
            SetMenuVisible(false);
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
            SetMenuVisible(false);
            if (scoreUI != null)
            {
                scoreUI.SetActive(true);
            }
        }

        public void OpenSettings()
        {
            SetMenuVisible(false);
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
            if (soundtrack != null && soundtrack.isPlaying)
            {
                soundtrack.Stop();
            }

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

        public void ReturnFromGameOver()
        {
            SetMenuVisible(true);
        }

        public void SaveHighScore()
        {
            TextMeshProUGUI playerNameText = GameObject.Find("PlayerText")?.GetComponent<TextMeshProUGUI>();
            if (playerNameText == null)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Player name text not found please enter a name to save your score.");
                return;
            }

            string playerName = playerNameText.text;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Player name is empty, please enter a name to save your score.");
                return;
            }

            if (playerRecords == null)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Cannot save high score; PlayerRecords not available.");
                return;
            }

            WordGame wordGame = FindFirstObjectByType<WordGame>();
            float time = wordGame != null ? wordGame.GetPlayTime() : Time.timeSinceLevelLoad;
            int score = CalculateScore(time);
            playerRecords.SaveRecord(playerName, score, time);
            ReturnFromGameOver();
        }
        
        private int CalculateScore(float time)
        {
            return Mathf.Max(0, Mathf.RoundToInt(1000 - time * 10));
        }

        public void ReturnToMenu()
        {
            Debug.Log($"{name} ({GetType().Name}): Returning to main menu.");
            SetMenuVisible(true);
            if (boardGenerator != null && !boardGenerator.gameObject.activeSelf)
            {
                boardGenerator.gameObject.SetActive(true);
            }
        }

        private void SetMenuVisible(bool show)
        {
            isMenuOpen = show;
            if (menuUI != null)
            {
                menuUI.SetActive(show);
            }

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(!show);
            }

            if (scoreUI != null)
            {
                scoreUI.SetActive(false);
            }
        }
    }
}