using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set; }

    private string highestLevelKey = "HighestLevelKey";

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

    public void SavePlayerLevel(int level)
    {
        PlayerPrefs.SetInt(highestLevelKey, level);
    }

    public int LoadPlayerLevel()
    {
        if (PlayerPrefs.HasKey(highestLevelKey))
        {
            return PlayerPrefs.GetInt(highestLevelKey);
        }
        else
        {
            return 1;
        }
    }
}
