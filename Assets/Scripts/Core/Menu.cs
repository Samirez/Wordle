using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private bool isMenuOpen = true;
    private Button startButton;
    private GameObject menuUI, scoreUI, settingsUI;

    public void Awake()
    {
        menuUI = GameObject.Find("MenuUI");
        if (menuUI == null)
        {
            menuUI = gameObject;
        }
        menuUI.SetActive(isMenuOpen);
        startButton = menuUI.transform.Find("StartButton").GetComponentInChildren<Button>();
        startButton.onClick.AddListener(StartGame);

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
    }

    public void StartGame()
    {
        isMenuOpen = false;
        menuUI.SetActive(isMenuOpen);
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
}
