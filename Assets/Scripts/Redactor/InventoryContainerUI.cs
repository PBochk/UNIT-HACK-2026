using UnityEngine;

public class InventoryContainerUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private RedactorDragAndDropManager dragAndDrop;
    
    public void Start()
    {
        if (inventory == null)
        {
            Debug.LogWarning("InventoryContainerUI: inventory is null");
            return;
        }
        
        if (dragAndDrop == null)
        {
            Debug.LogWarning("InventoryContainerUI: dragAndDrop is null");
            return;
        }
        
        var slots = GetComponentsInChildren<InventorySlot>();
        foreach (var slot in slots)
        {
            slot.Initialize(inventory,dragAndDrop);
        }
    }
}