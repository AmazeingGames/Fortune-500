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
        GameFlowManager.ChangeGameStateEventHandler += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameFlowManager.ChangeGameStateEventHandler -= HandleGameStateChange;
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
            case GameFlowManager.GameState.InMenu:
            case GameFlowManager.GameState.Paused:
                sceneEnvironment.SetActive(false);
            break;
            
            case GameFlowManager.GameState.Running:
                sceneEnvironment.SetActive(true);
                foreach (var menu in virtualMenus)
                    menu.SetActive(true);
            break;

        }
    }
}
