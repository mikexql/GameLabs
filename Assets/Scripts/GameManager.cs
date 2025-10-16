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

    public UnityEvent gameReset;

    private int score = 0;

    public int lastSessionScore;

    // loading control
    private Coroutine currentLoadRoutine = null;
    private string currentLoadingTarget = null;
    private bool loadCancelled = false;


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

    public void GameReset()
    {
        Debug.Log("Game Manager: Reset!");
        // reset score
        score = 0;
        SetScore(score);
        gameReset.Invoke();
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
        // cancel any existing load
        if (currentLoadRoutine != null) CancelLoad();
        currentLoadRoutine = StartCoroutine(LoadSceneCoroutine(sceneName, minDuration, false));
    }

    public void LoadSceneWithDelay(string sceneName, float minDuration, bool fromMainMenu)
    {
        if (currentLoadRoutine != null) CancelLoad();
        currentLoadRoutine = StartCoroutine(LoadSceneCoroutine(sceneName, minDuration, fromMainMenu));
    }

    public void ReloadCurrentScene(float minDuration = 0.5f)
    {
        if (currentLoadRoutine != null) CancelLoad();
        currentLoadRoutine = StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().name, minDuration, false));
    }

    public void LoadMainMenu(float minDuration = 0.5f)
    {
        if (currentLoadRoutine != null) CancelLoad();
        currentLoadRoutine = StartCoroutine(LoadSceneCoroutine("Main Menu", minDuration, false));
    }

    private System.Collections.IEnumerator LoadSceneCoroutine(string sceneName, float minDuration, bool fromMainMenu)
    {
        // Ensure gameplay timeScale is normal during loading UI
        Time.timeScale = 1f;
        string loadingSceneName = "Loading";
        currentLoadingTarget = sceneName;
        loadCancelled = false;
        if (fromMainMenu)
        {
            AsyncOperation loadLoading = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
            // Wait until the loading scene is loaded and active so its UI shows
            while (!loadLoading.isDone)
                yield return null;

            var loadedScene = SceneManager.GetSceneByName(loadingSceneName);
            if (loadedScene.IsValid())
                SceneManager.SetActiveScene(loadedScene);
            // Unload the Main Menu scene immediately to avoid overlapping UI and active-scene conflicts
            if (SceneManager.GetSceneByName("Main Menu").IsValid())
            {
                SceneManager.UnloadSceneAsync("Main Menu");
            }
        }

        float startTime = Time.unscaledTime;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (loadCancelled)
            {
                Debug.Log("GameManager: Load cancelled for " + sceneName);
                // Attempt to unload partially loaded scene and loading UI
                if (SceneManager.GetSceneByName(loadingSceneName).IsValid())
                    SceneManager.UnloadSceneAsync(loadingSceneName);

                if (SceneManager.GetSceneByName(sceneName).IsValid() && SceneManager.GetSceneByName(sceneName).isLoaded)
                    SceneManager.UnloadSceneAsync(sceneName);

                // cleanup state
                currentLoadRoutine = null;
                currentLoadingTarget = null;
                loadCancelled = false;
                yield break;
            }
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
        // clear load state
        currentLoadRoutine = null;
        currentLoadingTarget = null;
        loadCancelled = false;
    }

    // Public API for cancelling an in-progress load (e.g. from Loading UI)
    public void CancelLoad()
    {
        Debug.Log("GameManager: CancelLoad called");
        // mark cancellation; coroutine will clean up
        loadCancelled = true;
        // Ensure gameplay timeScale is restored
        Time.timeScale = 1f;
        // Attempt to unload loading UI immediately if present
        if (SceneManager.GetSceneByName("Loading").IsValid())
            SceneManager.UnloadSceneAsync("Loading");
        // If there's a pending target, try to unload it if already loaded
        if (!string.IsNullOrEmpty(currentLoadingTarget))
        {
            var sc = SceneManager.GetSceneByName(currentLoadingTarget);
            if (sc.IsValid() && sc.isLoaded)
                SceneManager.UnloadSceneAsync(currentLoadingTarget);
        }
        // Load the Main Menu immediately
        SceneManager.LoadScene("Main Menu");
        // reset tracking fields (coroutine will also clear them if still running)
        currentLoadingTarget = null;
    }

    // Expose load state for external callers
    public bool IsLoading
    {
        get { return currentLoadRoutine != null; }
    }
}