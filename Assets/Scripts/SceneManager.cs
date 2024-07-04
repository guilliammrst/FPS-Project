using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; set; }
    
    public bool gamePaused = false;
    public bool levelCleared = false;

    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject levelClearedMenu;

    public readonly string mainMenuScene = "MainMenu";
    public readonly string firstMapScene = "LevelOne";
    public readonly string secondMapScene = "LevelTwo";
    public readonly string thirdMapScene = "LevelThree";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != mainMenuScene)
        {
            FindPauseMenu();
            FindGameOverMenu();
            FindLevelClearedMenu();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gamePaused)
                {
                    UnpauseGame();
                }
                else
                {
                    PauseGame();
                }
            }

            if (FindObjectsOfType<Enemy>().Length == 0)
            {
                LevelCleared();
            }
        }
    }

    private void FindPauseMenu()
    {
        if (pauseMenu == null)
        {
            pauseMenu = GameObject.Find("PauseMenu");

            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);

                pauseMenu.transform.Find("ResumeButton").GetComponent<Button>().onClick.AddListener(UnpauseGame);
                pauseMenu.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(GoToMainMenuAndSave);
            }
        }
    }

    private void FindGameOverMenu()
    {
        if (gameOverMenu == null)
        {
            gameOverMenu = GameObject.Find("GameOverMenu");

            if (gameOverMenu != null)
            {
                gameOverMenu.SetActive(false);

                gameOverMenu.transform.Find("RestartButton").GetComponent<Button>().onClick.AddListener(() => LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
                gameOverMenu.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(GoToMainMenuAndSave);
            }
        }
    }

    private void FindLevelClearedMenu()
    {
        if (levelClearedMenu == null)
        {
            levelClearedMenu = GameObject.Find("LevelClearedMenu");

            if (levelClearedMenu != null)
            {
                levelClearedMenu.SetActive(false);

                levelClearedMenu.transform.Find("NextLevelButton").GetComponent<Button>().onClick.AddListener(GoToNextLevel);
                levelClearedMenu.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(GoToMainMenuAndSave);
            }
        }
    }

    public void LevelCleared()
    {
        Time.timeScale = 0;
        gamePaused = true;
        levelCleared = true;

        // Show level cleared menu
        levelClearedMenu.SetActive(true);

        // Show mouse pointer
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gamePaused = true;

        // Show game over menu
        gameOverMenu.SetActive(true);

        // Show mouse pointer
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        gamePaused = true;

        // Show pause menu
        pauseMenu.SetActive(true);

        // Show mouse pointer
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        gamePaused = false;

        // Unshow pause menu
        pauseMenu.SetActive(false);

        // Hide mouse pointer
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToMainMenuAndSave()
    {
        UnpauseGame();
        pauseMenu = null;
        gameOverMenu = null;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == firstMapScene && !levelCleared)
        {             
            SaveLoadManager.Instance.SavePlayerLevel(1);
        }
        else if ((UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == secondMapScene && !levelCleared) || (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == firstMapScene && levelCleared))
        {
            SaveLoadManager.Instance.SavePlayerLevel(2);
        }
        else if ((UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == thirdMapScene && !levelCleared) || (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == secondMapScene && levelCleared))
        {
            SaveLoadManager.Instance.SavePlayerLevel(3);
        }

        levelCleared = false;

        LoadScene("MainMenu");
    }

    public void GoToNextLevel()
    {
        UnpauseGame();
        pauseMenu = null;
        gameOverMenu = null;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == firstMapScene)
        {
            LoadScene(secondMapScene);
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == secondMapScene)
        {
            LoadScene(thirdMapScene);
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == thirdMapScene)
        {
            LoadScene(mainMenuScene);
        }
    }


    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        gamePaused = false;
        levelCleared = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
