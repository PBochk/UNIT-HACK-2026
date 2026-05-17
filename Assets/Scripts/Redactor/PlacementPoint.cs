using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacementPoint : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int Id { get; private set; }

    [SerializeField] private Image _image;
    [SerializeField] private DragPreview PreviewPrefab;
    
    private Sprite imageSpriteCache;
    private ObstacleAsset _currentObstacle;
    private ObstaclePlacement _placementAsset;
    private RedactorDragAndDropManager _dragAndDrop;
    private Inventory _inventory;
    private DragPreview _dragPreview;
    
    private GameObject _obstacleCache;
    private DragPreview _previewInstance;

    public void Start()
    {
        imageSpriteCache = _image.sprite;
    }
    
    public void Setup(int id, ObstaclePlacement placement, RedactorDragAndDropManager dragAndDropManager, Inventory inventory)
    {
        Id = id;
        _placementAsset = placement;
        _dragAndDrop = dragAndDropManager;
        _inventory = inventory;

        if (Id < _placementAsset.Obstacles.Count)
            _currentObstacle = _placementAsset.Obstacles[Id];
    }

    // --- НАЧАЛО ПЕРЕТАСКИВАНИЯ С ТОЧКИ ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentObstacle == null || _dragAndDrop.IsDragging) return;

        // Забираем предмет в руку "с карты" (не трогая инвентарь)
        _dragAndDrop.TakeFromMap(_currentObstacle);

        // Очищаем саму точку и данные в ScriptableObject
        _currentObstacle = null;
        _placementAsset.Obstacles[Id] = null;
        
        var canvas = GetComponentInParent<Canvas>();
        _previewInstance = Instantiate(PreviewPrefab, canvas.transform, false);
        _previewInstance.transform.SetAsLastSibling();
        _previewInstance.Initialize(_currentObstacle.Sprite);

        UpdateVisuals();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Обязательно оставляем пустым, чтобы работал OnEndDrag
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

    // --- ЗАВЕРШЕНИЕ ПЕРЕТАСКИВАНИЯ, НАЧАТОГО С ТОЧКИ ---
    public void OnEndDrag(PointerEventData eventData)
    {
        // Если мы подняли предмет с этой точки, потащили, но никуда не пристроили (бросили в пустоту)
        if (_dragAndDrop.IsDragging)
        {
            _dragAndDrop.ReturnToInventory();
            Debug.Log("Предмет снят с карты и возвращен в инвентарь.");
        }
        
        if (_previewInstance != null)
        {
            Destroy(_previewInstance.gameObject);
        }
    }

    // --- СБРОС ПРЕДМЕТА НА ЭТУ ТОЧКУ ---
    public void OnDrop(PointerEventData eventData)
    {
        if (!_dragAndDrop.IsDragging || _dragAndDrop.Holding == null) return;

        // Если на точке уже был другой предмет — возвращаем его в инвентарь
        if (_currentObstacle != null)
        {
            _inventory.Add(_currentObstacle);
        }

        // Принимаем новый предмет
        _currentObstacle = _dragAndDrop.Holding;
        _placementAsset.Obstacles[Id] = _currentObstacle;

        UpdateVisuals();

        // Успешно разместили! Сбрасываем флаг, чтобы OnEndDrag оригинального слота ничего не отменял
        _dragAndDrop.ClearHolding();
    }

    private void UpdateVisuals()
    {
        if(_obstacleCache != null)
        {
            Destroy(_obstacleCache);
        }
        
        // Включение/выключение графики препятствия на этой точке
        if (_currentObstacle != null)
        {
            //_image.enabled = false;
            _image.sprite = null;
            _image.color = new (1, 1, 1, 0);
            _obstacleCache = Instantiate(_currentObstacle.ObstaclePrefab, null);
            _obstacleCache.transform.position = transform.position;
            return;
        }
        
        _image.sprite = imageSpriteCache;
        _image.color = new (1, 1, 1, 1);
        _image.enabled = true;
    }
}