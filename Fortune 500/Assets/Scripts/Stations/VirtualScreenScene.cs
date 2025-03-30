using UnityEngine;
using static VirtualScreen;

public class VirtualScreenScene : MonoBehaviour
{
    [SerializeField] StationData VirtualScreenSceneData;

    private void OnEnable()
        => VirtualScreen.FindStationDataEventHandler += HandleFindStationData;

    private void OnDisable()
        => VirtualScreen.FindStationDataEventHandler -= HandleFindStationData;

    void HandleFindStationData(object sender, FindStationDataEventArgs e)
    {

    }



}
