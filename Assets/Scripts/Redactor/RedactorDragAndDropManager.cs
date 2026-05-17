using System;
using Unity.VisualScripting;
using UnityEngine;

public class RedactorDragAndDropManager : MonoBehaviour
{
    public bool IsDragging { get; private set; }
    public ObstacleAsset Holding { get; private set; }

    [SerializeField] private Inventory inventory;

    public event Action DragBegin; 
    public event Action DragEnd; 

    // Взять из инвентаря (кол-во уменьшается)
    public void TakeFromInventory(ObstacleAsset obstacle)
    {
        if (IsDragging || obstacle == null) return;

        if (inventory.TryRemove(obstacle))
        {
            Holding = obstacle;
            IsDragging = true;
            DragBegin?.Invoke();
        }
    }

    // Взять уже установленный на карте предмет (кол-во НЕ уменьшается)
    public void TakeFromMap(ObstacleAsset obstacle)
    {
        if (IsDragging || obstacle == null) return;

        Holding = obstacle;
        IsDragging = true;
        DragBegin?.Invoke();
    }
    
    // Возврат в инвентарь (принудительное увеличение кол-ва)
    public void ReturnToInventory()
    {
        if (!IsDragging) return;
        
        inventory.Add(Holding);
        ClearHolding();
    }

    public void ClearHolding()
    {
        Holding = null;
        IsDragging = false;
        DragEnd?.Invoke();
    }
}