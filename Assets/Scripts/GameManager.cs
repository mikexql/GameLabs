using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    private int score = 0;

    public int lastSessionScore;


    //Managed items
    // public GameObject player;
    // public GameObject EnemyManager;
    // public GameObject HUDManager;

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;
        SceneManager.activeSceneChanged += SceneSetup;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        Debug.Log("Game Manager: Restart!");
        // reset score
        score = 0;
        SetScore(score);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        Debug.Log("Game Manager: Score: " + score);
        SetScore(score);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }


    public void GameOver()
    {
        Debug.Log("Game Manager: Game Over!");
        lastSessionScore = score;
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }

    public void SceneSetup(Scene current, Scene next)
    {
        gameStart.Invoke();
        SetScore(score);
    }

    public void ResetLastSessionScore()
    {
        score = 0;
        lastSessionScore = 0;
        SetScore(score);
    }
    
    public void LoadSceneWithDelay(string sceneName, float minDuration = 1f)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, minDuration, false));
    }

    public void LoadSceneWithDelay(string sceneName, float minDuration, bool fromMainMenu)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, minDuration, fromMainMenu));
    }

    public void ReloadCurrentScene(float minDuration = 0.5f)
    {
        StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().name, minDuration, false));
    }

    public void LoadMainMenu(float minDuration = 0.5f)
    {
        StartCoroutine(LoadSceneCoroutine("Main Menu", minDuration, false));
    }

    private System.Collections.IEnumerator LoadSceneCoroutine(string sceneName, float minDuration, bool fromMainMenu)
    {
        // Ensure gameplay timeScale is normal during loading UI
        Time.timeScale = 1f;
        string loadingSceneName = "Loading";
        if (fromMainMenu)
        {
            AsyncOperation loadLoading = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
            // Wait until the loading scene is loaded and active so its UI shows
            while (!loadLoading.isDone)
                yield return null;

            var loadedScene = SceneManager.GetSceneByName(loadingSceneName);
            if (loadedScene.IsValid())
                SceneManager.SetActiveScene(loadedScene);
        }

        float startTime = Time.unscaledTime;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                float elapsed = Time.unscaledTime - startTime;
                float required = fromMainMenu ? Mathf.Max(3f, minDuration) : minDuration;
                if (elapsed >= required)
                {
                    op.allowSceneActivation = true;
                }
            }
            yield return null;
        }

        // If we loaded a dedicated loading scene, unload it now that the main
        // scene has been activated.
        if (fromMainMenu)
        {
            SceneManager.UnloadSceneAsync(loadingSceneName);
        }
    }
}