using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Assets")]
    [SerializeField] private ObstacleAsset obstacleAsset;
    [SerializeField] private Stat UnlockedStat;

    [Header("GO links")]
    [SerializeField] private Image Icon;
    [SerializeField] private TextMeshProUGUI AmountText;
    [SerializeField] private GameObject AmountBadge;
    [SerializeField] private Image InventoryItemImage;
    
    private bool _disabled = false;
    private Inventory _inventory;
    private RedactorDragAndDropManager dragAndDrop;

    public void Initialize(Inventory inventory, RedactorDragAndDropManager dragAndDrop)
    {
        _inventory = inventory;
        if (_disabled) return;
        if (!inventory.AmountHeld.ContainsKey(obstacleAsset))
        {
            Debug.LogWarning($"Inventory does not contain {obstacleAsset.Id} as name, adding it");
            inventory.AmountHeld.Add(obstacleAsset, 0);
        }

        inventory.OnInventoryUpdate += () =>
        {
            var amount = inventory.AmountHeld[obstacleAsset];
            UpdateAmount(amount);
        };
        var amount = inventory.AmountHeld[obstacleAsset];
        UpdateAmount(amount);
        InventoryItemImage.sprite = obstacleAsset.Sprite;
    }

    public void Start()
    {
        if (obstacleAsset == null)
            DisableSlot();
        if (UnlockedStat != null && UnlockedStat.Value == 0)
        {
            DisableSlot();
            UnlockedStat.OnValueChanged += () =>
            {
                if (UnlockedStat.Value > 0)
                    EnableSlot();
            };
        }
    }

    public void DisableSlot()
    {
        AmountBadge.SetActive(false);
        InventoryItemImage.gameObject.SetActive(false);
        _disabled = true;
    }

    public void EnableSlot()
    {
        AmountBadge.SetActive(true);
        InventoryItemImage.gameObject.SetActive(true);
        _disabled = false;
    }

    public void UpdateAmount(int amount)
    {
        AmountText.text = amount.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragAndDrop.TakeFromInventory(obstacleAsset);
        // Здесь можно включать/создавать визуальную иконку, следующую за мышью
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Метод пустой, но ОБЯЗАТЕЛЬНЫЙ для работы OnEndDrag.
        // Здесь обычно обновляют позицию иконки: icon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // КЛЮЧЕВАЯ ЛОГИКА: Если мы закончили тащить, а менеджер все еще IsDragging,
        // значит OnDrop на точках размещения НЕ сработал (бросили в пустоту или обратно в UI инвентаря)
        if (dragAndDrop.IsDragging)
        {
            dragAndDrop.ReturnToInventory();
            Debug.Log("Сброшено в пустоту: предмет возвращен в инвентарь.");
        }
        
        // Здесь уничтожаем/прячем визуальную иконку перетаскивания
    }
}