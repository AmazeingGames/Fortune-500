using TMPro;
using UnityEngine;

public class EnvironmentData : MonoBehaviour
{
    [field: SerializeField] public TextMeshPro RevenueText { get; private set; }
    [field: SerializeField] public TextMeshPro StrikesLeftText { get; private set; }
    [field: SerializeField] public TextMeshPro DayText {get; private set; }
}
