using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Button btnPlay;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject levelSelectionCanvas;
    [SerializeField] private Transform levelsGrid;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject gamePlayCanvas;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private TMP_Text scoreText;
    
    public void OnPlay()
    {
        gamePlayCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        winCanvas.SetActive(false);
        levelSelectionCanvas.SetActive(false);
        settingCanvas.SetActive(false);
    }

    public void OnMainMenu()
    {
        gamePlayCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        winCanvas.SetActive(false);
        settingCanvas.SetActive(false);
    }
    
    public void LoadLevelButtons()
    {
        // Clear any existing level buttons
        foreach (Transform child in levelsGrid)
        {
            Destroy(child.gameObject);
        }
        // Create a button for each level and add it to the levels grid
        for (int i = 0; i < LevelsManager.GetInstance().GetMaxUnlockedLevel(); i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, levelsGrid);
            Button button = buttonGO.GetComponent<Button>();
            int levelIndex = i; // Capture the value of 'i' for the lambda
            button.onClick.AddListener(() => GameManager.GetInstance().OnPlayIndexLevel(levelIndex)); 
            button.GetComponentInChildren<TMP_Text>().text = "Level " + (i + 1);
        }
    }

    public void OnSelecLevels()
    {
        levelSelectionCanvas.gameObject.SetActive(true);
    }

    public void OnWinLevel()
    {
        gamePlayCanvas.SetActive(false);

        if (winCanvas != null) 
        {
            winCanvas.SetActive(true);
        }
    }

    public void UpdateScore(int point)
    {
        scoreText.text = point.ToString();
    }
}
