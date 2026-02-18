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
        private GameManager gameManager;
        private AudioSource menuPanelAudio;
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

            settingsUI = GameObject.FindGameObjectWithTag("SettingsUI");

            if (settingsUI != null)
            {
                settingsUI.SetActive(false);
            }

            guessUI = GameObject.FindGameObjectWithTag("GuessUI");
            
            if (guessUI != null)
            {
                guessUI.SetActive(false);
            }

            gameOverUI = GameObject.FindGameObjectWithTag("GameOverUI");

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
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): GameManager not found; soundtrack control will be unavailable.");
            }

            menuPanelAudio = menuUI != null ? menuUI.GetComponent<AudioSource>() : null;
        }

        void Start()
        {
            playerRecords = FindFirstObjectByType<PlayerRecords>();
            gameManager?.StopSoundtrack();

            if (playerRecords == null)
            {
                Debug.LogWarning(
                    $"{name} ({GetType().Name}): PlayerRecords not found; high score storage will be unavailable.");
            }
        }

        public void StartGame()
        {
            gameManager?.PlaySoundtrack();

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
            PlayMenuPanelSound();
            SetMenuVisible(false);
            if (scoreUI != null)
            {
                scoreUI.SetActive(true);
            }
        }

        public void OpenSettings()
        {
            PlayMenuPanelSound();
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
            gameManager?.StopSoundtrack();

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
            string playerName = string.Empty;

            TMP_InputField playerNameInput = gameOverUI != null ? gameOverUI.GetComponentInChildren<TMP_InputField>(true) : null;
            if (playerNameInput != null)
            {
                playerName = playerNameInput.text;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                TextMeshProUGUI playerNameText = GameObject.Find("PlayerText")?.GetComponent<TextMeshProUGUI>();
                if (playerNameText != null)
                {
                    playerName = playerNameText.text;
                }
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Player";
            }

            try
            {
                if (playerRecords == null)
                {
                    Debug.LogWarning($"{name} ({GetType().Name}): Cannot save high score; PlayerRecords not available.");
                    return;
                }

                WordGame wordGame = FindFirstObjectByType<WordGame>();
                float time = wordGame != null ? wordGame.GetPlayTime() : Time.timeSinceLevelLoad;
                int score = CalculateScore(time);
                playerRecords.SaveRecord(playerName, score, time);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Failed to save high score; returning to menu anyway. {ex.Message}");
            }
            finally
            {
                ReturnFromGameOver();
            }
        }
        
        private int CalculateScore(float time)
        {
            return Mathf.Max(0, Mathf.RoundToInt(1000 - time * 10));
        }

        public void ReturnToMenu()
        {
            gameManager?.StopSoundtrack();

            Debug.Log($"{name} ({GetType().Name}): Returning to main menu.");
            PlayMenuPanelSound();
            SetMenuVisible(true);
            if (guessUI != null)
            {
                guessUI.SetActive(false);
            }

            if (boardGenerator != null)
            {
                boardGenerator.ClearBoard();
                boardGenerator.gameObject.SetActive(false);
            }
        }

        private void PlayMenuPanelSound()
        {
            if (menuPanelAudio == null || menuPanelAudio.clip == null)
            {
                return;
            }

            menuPanelAudio.PlayOneShot(menuPanelAudio.clip, menuPanelAudio.volume);
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

            if (settingsUI != null)
            {
                settingsUI.SetActive(false);
            }
        }
    }
}