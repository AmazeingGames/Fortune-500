using UnityEngine;

[ExecuteAlways]
public class EnvironmentSortingLayer : MonoBehaviour
{
    public enum SortingLayerNames { Default, Cursor, EnvironmentBack, Candidate, EnvironmentFront, UI}
    [SerializeField] SortingLayerNames mySortingLayerName;
    [SerializeField] int sortingOrder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null)
            return;

        renderer.sortingLayerName = $"{mySortingLayerName}";
        renderer.sortingOrder = sortingOrder ;
        Debug.Log($"Set sorting layer name to {mySortingLayerName} | RESULT: {renderer.sortingLayerName}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
