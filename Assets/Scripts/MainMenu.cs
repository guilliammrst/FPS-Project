using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text currentLevelText;

    void Start()
    {
       int currentLevel = SaveLoadManager.Instance.LoadPlayerLevel();
        currentLevelText.text = "Current Level: " + currentLevel;
    }

    public void StartNewGame()
    {
        SaveLoadManager.Instance.SavePlayerLevel(1);
        SceneManager.Instance.LoadScene(SceneManager.Instance.firstMapScene);
    }

    public void ContinueGame()
    {
        switch (SaveLoadManager.Instance.LoadPlayerLevel())
        {
            case 1:
                SceneManager.Instance.LoadScene(SceneManager.Instance.firstMapScene);
                break;
            case 2:
                SceneManager.Instance.LoadScene(SceneManager.Instance.secondMapScene);
                break;
            case 3:
                SceneManager.Instance.LoadScene(SceneManager.Instance.thirdMapScene);
                break;
            default:
                SceneManager.Instance.LoadScene(SceneManager.Instance.firstMapScene);
                break;
        }
    }

    public void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
