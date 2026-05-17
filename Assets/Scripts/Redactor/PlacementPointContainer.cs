using System.Collections.Generic;
using UnityEngine;

public class PlacementPointContainer : MonoBehaviour
{
    [SerializeField] private List<PlacementPoint> placementPoints = new();
    [SerializeField] private ObstaclePlacement placementAsset;
    [SerializeField] private RedactorDragAndDropManager dragAndDrop;
    [SerializeField] private Inventory inventory; 

    private void Start()
    {
        // 1. Гарантируем, что ScriptableObject имеет нужный размер списка
        placementAsset.Initialize(placementPoints.Count);

        // 2. Инициализируем каждую точку, внедряя зависимости
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i] != null)
            {
                placementPoints[i].Setup(i, placementAsset, dragAndDrop, inventory);
            }
        }
    }
    
    // Бонус: контекстное меню для автоматического сбора точек в инспекторе
    [ContextMenu("Find All Placement Points")]
    private void FindPoints()
    {
        placementPoints = new List<PlacementPoint>(GetComponentsInChildren<PlacementPoint>());
    }
}