using TMPro;
using UnityEngine;

public class EnvironmentData : Singleton<EnvironmentData>
{
    [field: SerializeField] public TextMeshPro RevenueText { get; private set; }
    [field: SerializeField] public TextMeshPro StrikesLeftText { get; private set; }
    [field: SerializeField] public TextMeshPro DayText {get; private set; }
}
