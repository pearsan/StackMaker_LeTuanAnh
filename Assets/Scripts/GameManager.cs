using System;
using UnityEngine;
using UnityEngine.Events;
using Application = UnityEngine.Device.Application;
using Screen = UnityEngine.Device.Screen;

public enum GameState { MainMenu, GamePlay, Finish, Revive, Setting }

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;

    public static void ChangeState(GameState state)
    {
        gameState = state;

        switch (gameState)
        {
            case GameState.MainMenu:
                UIManager.GetInstance().OnMainMenu();
                LevelsManager.GetInstance().DestroyLevel();
                break;
            case GameState.GamePlay:
                UIManager.GetInstance().OnPlay();
                break;
            case GameState.Finish:
                UIManager.GetInstance().OnWinLevel();
                LevelsManager.GetInstance().OnWin();
                break;
        }
    }

    public bool IsState(GameState state) => gameState == state;

    private void Awake()
    {
        //tranh viec nguoi choi cham da diem vao man hinh
        Input.multiTouchEnabled = false;
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        //tranh viec tat man hinh
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //xu tai tho
        int maxScreenHeight = 1280;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }
    }

    private void Start()
    {
        LevelsManager.GetInstance().LoadLevels();
        UIManager.GetInstance().LoadLevelButtons();
        ChangeState(GameState.MainMenu);
    }

    public void OnMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void OnPlay()
    {
        ChangeState(GameState.GamePlay);
        LevelsManager.GetInstance().LoadCurrentLevel();
    }

    public void OnPlayIndexLevel(int index)
    {
        Debug.Log(index);
        ChangeState(GameState.GamePlay);
        LevelsManager.GetInstance().LoadLevel(index);
    }

    public void OnNextLevel()
    {
        ChangeState(GameState.GamePlay);
        LevelsManager.GetInstance().NextLevel();
    }

    public void OnRetry()
    {
        ChangeState(GameState.GamePlay);
        LevelsManager.GetInstance().Retry();

    }

    public void OnSetting()
    {
        ChangeState(GameState.Setting);
    }

    public void OnExitSetting()
    {
        ChangeState(GameState.GamePlay);

    }
    public void OnFinish()
    {
        ChangeState(GameState.Finish);
    }
}