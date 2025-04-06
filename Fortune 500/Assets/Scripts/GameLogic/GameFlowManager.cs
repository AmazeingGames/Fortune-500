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
    public enum GameAction { None, EnterMainMenu, PlayGame, PauseGame, ResumeGame, LoseGame }

    public GameState MyCurrentState { get; private set; }
    public GameAction MyLastGameAction { get; private set; }

    public bool HasFinishedDay { get; private set; }
    public bool HasStartedWork { get; private set; }
    public LevelData LevelData { get; private set; }

    public static EventHandler<GameStateChangeEventArgs> GameStateChangeEventHandler;
    public static EventHandler<PerformGameActionEventArgs> PerformGameActionEventHandler;
    void OnEnable()
    {
        UIButton.UIInteractEventHandler += HandleUIInteract;
        CandidateHandler.ReviewedCandidateEventHandler += HandleReviewCandidate;
    }

    void OnDisable()
    {
        UIButton.UIInteractEventHandler += HandleUIInteract;
        CandidateHandler.ReviewedCandidateEventHandler -= HandleReviewCandidate;
    }

    // Start is called before the first frame update
    void Start()
        => PerformGameAction(GameAction.EnterMainMenu);

    private void Update()
    {
        // In the future, I would like the game to acknowledge this, and be able to smoothly transition between the 2 quickly
        if (Input.GetKeyDown(pauseKey) && !ScreenTransitions.IsPlayingTransitionAnimation)
        {
            switch (MyCurrentState)
            {
                case GameState.Running:
                    PerformGameAction(GameAction.PauseGame);
                break;

                case GameState.Paused:
                    PerformGameAction(GameAction.ResumeGame);
                break;
            }
        }
    }

    void HandleReviewCandidate(object sender, ReviewedCandidateEventArgs e)
    {
        if (e.DidLose)
            PerformGameAction(GameAction.LoseGame);
    }

    /// <summary> Performs a game action given from a UI button. </summary>
    void HandleUIInteract(object sender, UIButton.UIInteractEventArgs e)
    {
        if (e.buttonEvent != UIButton.EventType.GameAction || e.buttonInteraction != UIButton.UIInteractionTypes.Click)
            return;

        PerformGameAction(e.actionToPerform);
    }

    /// <summary> Informs listerners of a game action and updates the game state accordingly. </summary>
    /// <param name="action"> The game action to perform. </param>
    /// <param name="levelToLoad"> If we should load a level, otherwise leave at -1. </param>
    // Update game state in response to menu changes
    void PerformGameAction(GameAction action)
    {
        if (action == GameAction.None)
        {
            Debug.LogWarning("Cannont run comand 'none'.");
            return;
        }

        // Debug.Log($"Performed game action: {action}");
        MyLastGameAction = action;
        OnPerformGameAction(action);

        // Updates game state to fit the action
        switch (action)
        {
            case GameAction.EnterMainMenu:
                OnGameStateChange(GameState.InMenu);
            break;

            case GameAction.ResumeGame:
            case GameAction.PlayGame:
                OnGameStateChange(GameState.Running);
            break;

            case GameAction.PauseGame:
                OnGameStateChange(GameState.Paused);
            break;
        }
    }

    void OnPerformGameAction(GameAction action)
        => PerformGameActionEventHandler?.Invoke(this, new(this, action));

    /// <summary> Informs listeners on how to align with the current state of the game. </summary>
    /// <param name="newState"> The state of the game to update to. </param>
    void OnGameStateChange(GameState newState)
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

        GameStateChangeEventHandler?.Invoke(this, new(this, newState, previousState));
    }
}

public class PerformGameActionEventArgs : EventArgs
{
    public readonly GameFlowManager gameManager;
    public readonly GameAction gameAction;

    public PerformGameActionEventArgs(GameFlowManager gameManager, GameAction gameAction)
    {
        this.gameManager = gameManager;
        this.gameAction = gameAction;
    }
}

public class GameStateChangeEventArgs : EventArgs
{
    public readonly GameFlowManager gameManager;
    public readonly GameState newState;
    public readonly GameState previousState;

    public GameStateChangeEventArgs(GameFlowManager gameManager, GameState newState, GameState previousState)
    {
        this.gameManager = gameManager;
        this.newState = newState;
        this.previousState = previousState;
    }
}