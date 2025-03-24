using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PinkSlip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI mistake;
    [SerializeField] TextMeshProUGUI conclusion;

    [SerializeField] Canvas parentCanvas;

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerEventData = data as PointerEventData;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)parentCanvas.transform, pointerEventData.position, parentCanvas.worldCamera, out position);

        transform.position = parentCanvas.transform.TransformPoint(position);
    }

    void Initialize(string title, string mistake, string conclusion)
    {
        this.title.text = title;
        this.mistake.text = mistake;
        this.conclusion.text = conclusion;
    }
}
