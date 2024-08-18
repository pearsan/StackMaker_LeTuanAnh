using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : Singleton<LevelsManager>
{
    public List<GameObject> levels = new List<GameObject>();
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";
    private const string UNLOCKED_LEVEL_KEY = "MaxLevel";
    private int currentLevelIndex = 0;
    private Level currentLevel;
    public void LoadLevels()
    {
        // Load all level prefabs from the Resources folder
        GameObject[] levelPrefabs = Resources.LoadAll<GameObject>("Levels");

        // Add the loaded prefabs to the levels list
        foreach (GameObject prefab in levelPrefabs)
        {
            levels.Add(prefab);
        }
    }

    public void LoadCurrentLevel()
    {
        // Check if a current level is saved in PlayerPrefs
        currentLevelIndex = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0);

        // If the current level is null, load the first level
        if (currentLevelIndex >= levels.Count || currentLevelIndex < 0)
        {
            currentLevelIndex = 0;
            SaveCurrentLevel(currentLevelIndex);
        }

        // Instantiate the current level
        LoadLevel(currentLevelIndex);
    }

    public void SaveCurrentLevel(int index)
    {
        currentLevelIndex = index;
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, index);
        int maxUnlockedLevel = PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 0);
        if (index >= maxUnlockedLevel)
            PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, index);
    }

    public void OnWin()
    {
        int maxUnlockedLevel = PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 0);
        if (maxUnlockedLevel == currentLevelIndex)
            maxUnlockedLevel += 1;
        if (maxUnlockedLevel >= levels.Count - 1)
            maxUnlockedLevel = levels.Count - 1;
        PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, maxUnlockedLevel);
    }

    public GameObject GetLevel(int index)
    {
        if (!(index >= 0 && index < levels.Count))
        {
            Debug.LogError("Invalid level index: " + index);
            index = 0;
        }
        SaveCurrentLevel(index);
        return levels[index];
    }

    public int GetMaxUnlockedLevel()
    {
        int max = PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 0);
        return max += 1;
    }
    
    public void LoadLevel(int index)
    {
        // Instantiate the selected level
        currentLevel = Instantiate(GetLevel(index), transform.position, Quaternion.identity).GetComponent<Level>();
        currentLevel.OnInit();
    }

    public void Retry()
    {
        DestroyLevel();
        LoadLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        DestroyLevel();
        int index = currentLevelIndex + 1;
        LoadLevel(index);
    }

    public void DestroyLevel()
    {
        if (currentLevel != null)
        {
            currentLevel.OnDespawn();
            Destroy(currentLevel.gameObject);
        }
    }
}