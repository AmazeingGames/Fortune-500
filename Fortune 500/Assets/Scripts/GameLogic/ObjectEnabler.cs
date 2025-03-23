using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjectEnabler : MonoBehaviour
{
    [SerializeField] GameObject sceneEnvironment;
    [SerializeField] List<GameObject> virtualMenus;

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
        if (sceneEnvironment == null)
        {
            Debug.LogWarning("Environment is null");
            return;
        }
        switch (e.newState)
        {
            case GameManager.GameState.InMenu:
            case GameManager.GameState.Paused:
                sceneEnvironment.SetActive(false);
            break;
            
            case GameManager.GameState.Running:
                sceneEnvironment.SetActive(true);
                foreach (var menu in virtualMenus)
                    menu.SetActive(true);
            break;

        }
    }
}
