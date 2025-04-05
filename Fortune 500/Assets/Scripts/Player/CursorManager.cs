using UnityEditor;
using UnityEngine;
using static GameFlowManager;

public class CursorManager : MonoBehaviour
{
    bool enableCursorOnResume;
    private void OnEnable()
    {
        GameFlowManager.GameStateChangeEventHandler += HandleGameStateChange;

        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnected;
    }

    private void OnDisable()
    {
        GameFlowManager.GameStateChangeEventHandler -= HandleGameStateChange;

        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnected;
    }

    void HandleGameStateChange(object sender, GameStateChangeEventArgs e)
    {
        switch (e.newState)
        {
            case GameState.Loading:
            case GameState.Paused:
            case GameState.InMenu:
                Cursor.visible = true;
            break;

            case GameState.Running:
                Cursor.visible = enableCursorOnResume;
            break;
        }
    }

    void HandleInterfaceConnected(object sender, InterfaceConnectedEventArgs e)
    {
        switch (e.myInteractionType)
        {
            case FocusStation.InteractionType.Connect:
                enableCursorOnResume = true;
                Cursor.visible = true;
            break;
            
            case FocusStation.InteractionType.Disconnect:
                enableCursorOnResume = false;
                Cursor.visible = false;    
            break;
            
            case FocusStation.InteractionType.DoNothing:
            break;
        }
    }
}
