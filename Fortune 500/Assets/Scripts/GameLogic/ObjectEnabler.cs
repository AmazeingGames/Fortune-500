using System;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
    [SerializeField] GameObject environment;

    private void OnEnable()
    {
        GameManager.GameStateChangeEventHandler += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.GameStateChangeEventHandler -= HandleGameStateChange;
    }

    void HandleGameStateChange(object sender, GameStateChangeEventArgs e)
    {
        switch (e.newState)
        {
            case GameManager.GameState.InMenu:
            case GameManager.GameState.Paused:
                environment.SetActive(false);
            break;
            
            case GameManager.GameState.Running:
                environment.SetActive(true);
            break;
        }
    }
}
