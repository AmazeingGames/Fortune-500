using UnityEngine;
using static VirtualScreen;

public class VirtualScreenScene : MonoBehaviour
{
    [SerializeField] VirtualScreenSceneData VirtualScreenSceneData;

    private void OnEnable()
  => FindScreenDataEventHandler += HandleFindScreenData;

    private void OnDisable()
      => FindScreenDataEventHandler -= HandleFindScreenData;


}
