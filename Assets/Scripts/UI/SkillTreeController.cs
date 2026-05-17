using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTreeController : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("Zoom Settings")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float zoomSensitivity = 0.1f;

    [Header("Drag Settings")]
    // Ограничение перемещения (в пикселях от центра)
    [SerializeField] private Vector2 maxPanOffset = new Vector2(1000f, 1000f);

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Реализация перетаскивания (Drag)
    public void OnDrag(PointerEventData eventData)
    {
        // Сдвигаем контейнер на дельту движения мыши/пальца
        Vector3 newPosition = rectTransform.localPosition + (Vector3)eventData.delta;

        // Ограничиваем перемещение по осям X и Y
        newPosition.x = Mathf.Clamp(newPosition.x, -maxPanOffset.x, maxPanOffset.x);
        newPosition.y = Mathf.Clamp(newPosition.y, -maxPanOffset.y, maxPanOffset.y);

        rectTransform.localPosition = newPosition;
    }

    // Реализация приближения/отдаления (Zoom колесиком мыши)
    public void OnScroll(PointerEventData eventData)
    {
        // Вычисляем новый масштаб на основе инпута колесика
        float zoomAmount = eventData.scrollDelta.y * zoomSensitivity;
        Vector3 newScale = rectTransform.localScale + Vector3.one * zoomAmount;

        // Ограничиваем масштаб минимальным и максимальным значениями
        newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
        newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
        newScale.z = 1f; // Для 2D UI Z-координата всегда 1

        rectTransform.localScale = newScale;
    }
}