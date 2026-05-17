using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacementPoint : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int Id { get; private set; }

    [SerializeField] private Image _image;
    
    private ObstacleAsset _currentObstacle;
    private ObstaclePlacement _placementAsset;
    private RedactorDragAndDropManager _dragAndDrop;
    private Inventory _inventory;
    private DragPreview _dragPreview;
    
    private GameObject _obstacleCache;

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

        UpdateVisuals();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Обязательно оставляем пустым, чтобы работал OnEndDrag
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
        // Включение/выключение графики препятствия на этой точке
        if (_currentObstacle != null)
        {
            _image.enabled = false;
            _obstacleCache = Instantiate(_currentObstacle.ObstaclePrefab, null);
            _obstacleCache.transform.position = transform.position;
            return;
        }
        
        if(_obstacleCache != null)
        {
            Destroy(_obstacleCache);
        }
        _image.enabled = true;
    }
}