using UnityEngine;

public class StationCanvas : MonoBehaviour
{
    [SerializeField] PlayerFocus.Station myStation;
    [SerializeField] Canvas canvas;

    private void OnEnable()
        => FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnect;

    private void OnDisable()
        => FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnect;

    void HandleInterfaceConnect(object sender, InterfaceConnectedEventArgs e)
    {
        switch (e.myInteractionType)
        {
            case FocusStation.InteractionType.Connect:
                if (e.myStation != myStation)
                    canvas.enabled = false;
            break;

            case FocusStation.InteractionType.Disconnect:
                canvas.enabled = true;
            break;
        }
    }
}