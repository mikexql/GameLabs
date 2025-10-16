using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public Button startButton;
    public Button resetLastScoreButton;
    public TextMeshProUGUI mainMenuScoreText;

    void Awake()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStart.AddListener(OnGameStartFromManager);
            GameManager.instance.scoreChange.AddListener(OnScoreChanged);
        }
    }

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButton);

        if (resetLastScoreButton != null)
            resetLastScoreButton.onClick.AddListener(OnResetLastScoreButton);

        RefreshMenuScore();
        // Ensure main menu canvas is active if this scene is the main menu.
        if (mainMenuCanvas != null)
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
                mainMenuCanvas.SetActive(true);
        }
        // Subscribe to scene load so the menu can react when returning from gameplay
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStart.RemoveListener(OnGameStartFromManager);
            GameManager.instance.scoreChange.RemoveListener(OnScoreChanged);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnGameStartFromManager()
    {
        // Hide main menu when gameplay starts
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
    }

    private void OnScoreChanged(int newScore)
    {
        // Nothing for now, but keep handler in case menu reflects live score
    }

    public void OnStartButton()
    {
        Debug.Log("MainMenuManager: Start Button Clicked");
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadSceneWithDelay("World 1-1", 1.0f, true);
        }
    }

    public void OnLoadingBackToMenu()
    {
        Debug.Log("MainMenuManager: Loading Main Menu");
        // Activate local canvas (if any) and request cancelling the current load.
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (GameManager.instance != null)
        {
            GameManager.instance.CancelLoad();
        }
    }

    public void OnResetLastScoreButton()
    {
        Debug.Log("MainMenuManager: Reset Last Score Button Clicked");
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetLastSessionScore();
            RefreshMenuScore();
        }
    }

    public void RefreshMenuScore()
    {
        if (mainMenuScoreText == null) return;

        int last = 0;
        if (GameManager.instance != null)
        {
            last = GameManager.instance.lastSessionScore;
        }

        mainMenuScoreText.text = "Last Score: " + last.ToString();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(true);
            RefreshMenuScore();
        }
    }
}
