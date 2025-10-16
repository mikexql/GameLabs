using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject inGameCanvas;
    public GameObject mainMenuCanvas;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI mainMenuScoreText;

    public Button startButton;
    public Button resetLastScoreButton;

    // Start is called before the first frame update
    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButton);
        }
        if (resetLastScoreButton != null)
        {
            resetLastScoreButton.onClick.AddListener(OnResetLastScoreButton);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        // other instructions
        GameManager.instance.gameStart.AddListener(GameStart);
        GameManager.instance.gameOver.AddListener(GameOver);
        GameManager.instance.gameRestart.AddListener(GameStart);
        GameManager.instance.scoreChange.AddListener(SetScore);
    }

    public void GameStart()
    {   
        Debug.Log("HUD Manager: Game Start!");
        gameOverCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        inGameCanvas.SetActive(true);
    }

    public void SetScore(int score)
    {   
        Debug.Log("HUD Manager: Set Score: " + score);
        scoreText.text = "Score: " + score.ToString();
        finalScoreText.text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        Debug.Log("HUD Manager: Game Over!");
        gameOverCanvas.SetActive(true);
        inGameCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        RefreshMenuScore();
    }

    public void OnStartButton()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        GameManager.instance.LoadSceneWithDelay("World 1-1", 1.0f, true);
        
    }

    public void OnLoadingBackToMenu()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (inGameCanvas != null) inGameCanvas.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }

        public void OnResetLastScoreButton()
    {
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
    

}
