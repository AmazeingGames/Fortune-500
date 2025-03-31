using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameFlowManager;

/// <summary>
/// 
/// </summary>
public class GameFlowManager : Singleton<GameFlowManager>
{
    readonly KeyCode pauseKey = KeyCode.Escape;

    public enum GameState { None, InMenu, Running, Paused, Loading }
    public enum GameAction { None, EnterMainMenu, PlayGame, StartDay, PauseGame, ResumeGame, RestartDay, LoadNextDay, FinishDay, LoseGame, StartWork }

    public GameState MyCurrentState { get; private set; }
    public GameAction MyLastGameAction { get; private set; }

    public bool HasFinishedDay { get; private set; }
    public bool HasStartedWork { get; private set; }
    public LevelData LevelData { get; private set; }

    public static EventHandler<GameStateChangeEventArgs> ChangeGameStateEventHandler;
    public static EventHandler<GameActionEventArgs> PerformActionEventHandler;

    private EventInstance AmbianceSound;

    void OnEnable()
    {
        SlotMachineButton.SlotsInteractEventHandler += HandleSlotsInteract;
        ScenesManager.BeatLastLevelEventHandler += HandleBeatLastLevel;
        UIButton.UIInteractEventHandler += HandleUIInteract;
        LevelData.LoadLevelData += HandleLoadLevelData;
    }

    void OnDisable()
    {
        SlotMachineButton.SlotsInteractEventHandler -= HandleSlotsInteract;
        ScenesManager.BeatLastLevelEventHandler -= HandleBeatLastLevel;
        LevelData.LoadLevelData -= HandleLoadLevelData;
        UIButton.UIInteractEventHandler += HandleUIInteract;
    }

    // Start is called before the first frame update
    void Start()
    {
        PerformGameAction(GameAction.EnterMainMenu);
    }

    private void Update()
    {
        // In the future, I would like the game to acknowledge this, and be able to smoothly transition between the 2 quickly
        if (Input.GetKeyDown(pauseKey) && !ScreenTransitions.IsPlayingTransitionAnimation)
        {
            switch (MyCurrentState)
            {
                case GameState.Running:
                    if (MyLastGameAction == GameAction.FinishDay)
                        return;
                    PerformGameAction(GameAction.PauseGame);
                break;

                case GameState.Paused:
                    PerformGameAction(GameAction.ResumeGame);
                break;
            }
        }
    }

    void HandleSlotsInteract(object sender, EventArgs e)
    {
        if (HasStartedWork)
        {
            Debug.Log("Day has already started!");
            return;
        }
        PerformGameAction(GameAction.StartWork);
    }

    /// <summary> Performs a game action given from a UI button. </summary>
    void HandleUIInteract(object sender, UIButton.UIInteractEventArgs e)
    {
        if (e.buttonEvent != UIButton.UIEventTypes.GameAction || e.buttonInteraction != UIButton.UIInteractionTypes.Click)
            return;

        PerformGameAction(e.actionToPerform, e.levelToLoad);
    }

    /// <summary> Updates the game to end when we beat the last level. </summary>
    void HandleBeatLastLevel(object sender, EventArgs e)
        => PerformGameAction(GameAction.LoseGame);

    /// <summary> Saves level data when we finish loading a new level </summary>
    void HandleLoadLevelData(object sender, LevelData.LoadLevelDataEventArgs e)
    {
        if (LevelData == e.levelData && !e.isLoadingIn)
        {
            Debug.Log("Set level data null");
            LevelData = null;
            return;
        }

        if (LevelData != e.levelData && e.isLoadingIn)
        {
            Debug.Log("Set new level data");
            LevelData = e.levelData;
        }
        else
            Debug.Log("Did nothing to level data");
    }

    /// <summary> Informs listerners of a game action and updates the game state accordingly. </summary>
    /// <param name="action"> The game action to perform. </param>
    /// <param name="levelToLoad"> If we should load a level, otherwise leave at -1. </param>
    // Update game state in response to menu changes
    void PerformGameAction(GameAction action, int levelToLoad = -1)
    {
        if (action == GameAction.None)
        {
            Debug.LogWarning("Cannont run comand 'none'.");
            return;
        }

        Debug.Log($"Performed game action: {action}");
        MyLastGameAction = action;
        OnPerformGameAction(action, levelToLoad);

        // Start day is only performed on game start
        
        switch (action)
        {
            case GameAction.PlayGame:
                PerformGameAction(GameAction.StartDay);
                AmbianceSound.start();
            break;

            case GameAction.StartDay:
            case GameAction.RestartDay:
            case GameAction.LoadNextDay:
                HasFinishedDay = false;
            break;

            case GameAction.StartWork:
                HasStartedWork = true;
            break;

            case GameAction.FinishDay:
                HasFinishedDay = true;
                HasStartedWork = false;
            break;
        }

        // Updates game state to fit the action
        switch (action)
        {
            case GameAction.EnterMainMenu:
            case GameAction.FinishDay:
            case GameAction.LoseGame:
                OnGameStateChange(GameState.InMenu);
            break;

            case GameAction.StartDay:
            case GameAction.ResumeGame:
            case GameAction.RestartDay:
            case GameAction.LoadNextDay:
            case GameAction.StartWork:
                AmbianceSound.start();
                OnGameStateChange(GameState.Running);
            break;

            case GameAction.PauseGame:
                AmbianceSound.stop(STOP_MODE.ALLOWFADEOUT);
                OnGameStateChange(GameState.Paused);
            break;
        }
    }

    void OnPerformGameAction(GameAction action, int levelToLoad)
        => PerformActionEventHandler?.Invoke(this, new(this, action, levelToLoad));

    /// <summary> Informs listeners on how to align with the current state of the game. </summary>
    /// <param name="newState"> The state of the game to update to. </param>
    void OnGameStateChange(GameState newState, int levelToLoad = -1)
    {
        if (newState == GameState.None)
        {
            Debug.LogWarning("Cannont update game state to 'none'.");
            return;
        }
        else if (newState == MyCurrentState)
        {
            Debug.LogWarning($"Cannont update game state to its own state ({newState}).");
            return;
        }

        var previousState = MyCurrentState;
        MyCurrentState = newState;

        ChangeGameStateEventHandler?.Invoke(this, new(this, newState, previousState, levelToLoad));
    }
}

public class GameActionEventArgs : EventArgs
{
    public readonly GameFlowManager gameManager;
    public readonly GameAction gameAction;
    public readonly int levelToLoad;

    public GameActionEventArgs(GameFlowManager gameManager, GameAction gameAction, int levelToLoad)
    {
        this.gameManager = gameManager;
        this.gameAction = gameAction;
        this.levelToLoad = levelToLoad;
    }
}

public class GameStateChangeEventArgs : EventArgs
{
    public readonly GameFlowManager gameManager;
    public readonly GameState newState;
    public readonly GameState previousState;
    public readonly int levelToLoad;

    public GameStateChangeEventArgs(GameFlowManager gameManager, GameState newState, GameState previousState, int levelToLoad)
    {
        this.gameManager = gameManager;
        this.newState = newState;
        this.previousState = previousState;
        this.levelToLoad = levelToLoad;
    }
}