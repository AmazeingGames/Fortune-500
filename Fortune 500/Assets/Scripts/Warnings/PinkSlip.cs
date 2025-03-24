using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using static FocusStation;
using Unity.VisualScripting;

public class PinkSlip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI mistake;
    [SerializeField] TextMeshProUGUI conclusion;

    Vector3 mouseOffset;
    float mZCoord;
    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mouseOffset = gameObject.transform.position - GetMouseWorldPosition();
    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mouseOffset;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition; 
        mousePoint.z = mZCoord;

        return Camera.main.WorldToScreenPoint(mousePoint);
    }

    public void Initialize(string title, string mistake, string conclusion)
    {
        this.title.text = title;
        this.mistake.text = mistake;
        this.conclusion.text = conclusion;
    }
}
