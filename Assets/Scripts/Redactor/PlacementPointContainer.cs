using System.Collections.Generic;
using UnityEngine;

public class PlacementPointContainer : MonoBehaviour
{
    // [SerializeField] private List<PlacementPoint> placementPoints = new();
    [SerializeField] private ObstaclePlacement placementAsset;
    [SerializeField] private RedactorDragAndDropManager dragAndDrop;
    [SerializeField] private Inventory inventory; 

    private void Start()
    {
        var points = GetComponentsInChildren<PlacementPoint>();
        placementAsset.Initialize(points.Length);
        var i = 0;
        foreach (var point in points)
        {
            point.Setup(i, placementAsset, dragAndDrop, inventory);
            i++;
        }
    }
}