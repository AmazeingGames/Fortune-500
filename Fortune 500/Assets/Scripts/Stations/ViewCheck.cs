using UnityEngine;

public class ViewCheck : MonoBehaviour
{
    [SerializeField] float playerLookTolerance;
    [SerializeField] float playerCameraTolerance;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerCamera;
    public bool IsPlayerFacing { get; private set; }
    public bool IsCameraFacing { get; private set; }

    float playerDOT;
    float cameraDOT;

    // Update is called once per frame
    void Update()
    {
        Vector3 playerDirection = (transform.position - player.transform.position).normalized;
        playerDOT = Vector3.Dot(player.transform.forward, playerDirection);
        IsPlayerFacing = playerDOT >= 1 - playerLookTolerance;

        Vector3 playerCameraDirection = (transform.position - playerCamera.transform.position).normalized;
        cameraDOT = Vector3.Dot(playerCamera.transform.forward, playerCameraDirection);
        IsCameraFacing = cameraDOT >= 1 - playerCameraTolerance;
    }
}
