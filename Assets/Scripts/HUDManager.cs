using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : Singleton<HUDManager>
{
    public GameObject gameOverCanvas;
    public GameObject inGameCanvas;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameStart()
    {   
        Debug.Log("HUD Manager: Game Start!");
        gameOverCanvas.SetActive(false);
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
    }
}
