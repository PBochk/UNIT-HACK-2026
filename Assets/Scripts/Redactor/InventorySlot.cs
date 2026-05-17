using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Assets")]
    [SerializeField] private ObstacleAsset obstacleAsset;
    [SerializeField] private Stat UnlockedStat;
    [SerializeField] private DragPreview PreviewPrefab;

    [Header("GO links")]
    [SerializeField] private Image Icon;
    [SerializeField] private TextMeshProUGUI AmountText;
    [SerializeField] private GameObject AmountBadge;
    [SerializeField] private Image InventoryItemImage;
    
    private bool _disabled = false;
    private Inventory _inventory;
    private RedactorDragAndDropManager _dragAndDrop;
    private DragPreview _previewInstance;

    public void Initialize(Inventory inventory, RedactorDragAndDropManager dragAndDrop)
    {
        _inventory = inventory;
        _dragAndDrop  = dragAndDrop;
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
        _dragAndDrop.TakeFromInventory(obstacleAsset);
        // Здесь можно включать/создавать визуальную иконку, следующую за мышью
        if (!_dragAndDrop.IsDragging) return;
        var canvas = GetComponentInParent<Canvas>();
        _previewInstance = Instantiate(PreviewPrefab, canvas.transform, false);
        _previewInstance.transform.SetAsLastSibling();
        _previewInstance.Initialize(obstacleAsset.Sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_previewInstance != null)
        {
            // Получаем RectTransform превью
            RectTransform previewRect = _previewInstance.GetComponent<RectTransform>();
            // Получаем RectTransform родителя (Канваса)
            RectTransform parentRect = _previewInstance.transform.parent as RectTransform;
        
            // Получаем Canvas, чтобы узнать, какая камера за него отвечает
            Canvas canvas = _previewInstance.GetComponentInParent<Canvas>();
            Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

            // Магическая функция Unity, которая правильно рассчитывает позицию
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, cam, out Vector2 localPoint))
            {
                previewRect.anchoredPosition = localPoint;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // КЛЮЧЕВАЯ ЛОГИКА: Если мы закончили тащить, а менеджер все еще IsDragging,
        // значит OnDrop на точках размещения НЕ сработал (бросили в пустоту или обратно в UI инвентаря)
        if (_dragAndDrop.IsDragging)
        {
            _dragAndDrop.ReturnToInventory();
            Debug.Log("Сброшено в пустоту: предмет возвращен в инвентарь.");
        }
        
        // Здесь уничтожаем/прячем визуальную иконку перетаскивания
        if (_previewInstance != null)
        {
            //Destroy(_previewInstance.gameObject);
        }
    }
}