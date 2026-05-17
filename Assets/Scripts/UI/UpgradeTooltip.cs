using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Добавлено для LayoutRebuilder

public class UpgradeTooltip : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject tooltipPanel; 
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("Price System Settings")]
    [SerializeField] private Transform priceContainer;    
    [SerializeField] private PriceElement pricePrefab;     
    [SerializeField] private TextMeshProUGUI infoPriceText; 

    [Header("Positioning Settings")]
    [SerializeField] private bool followMouse = true;
    // Офсет теперь работает в локальных координатах Canvas (например, 20, -20)
    [SerializeField] private Vector2 offset = new Vector2(20f, -20f); 

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private RectTransform parentRectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        
        if (transform.parent != null)
        {
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
        }
        
        Hide(); 
    }

    private void Update()
    {
        if (followMouse && tooltipPanel.activeSelf)
        {
            UpdatePosition();
        }
    }

    public void Show(Upgrade upgrade)
    {
        if (upgrade == null) return;
        tooltipPanel.SetActive(true);

        if (titleText != null) titleText.text = upgrade.Stat.DisplayName;
        if (descriptionText != null) descriptionText.text = upgrade.Stat.DisplayDescription;

        int currentLevel = upgrade.Level;
        int maxLevel = upgrade.Levels.Count;

        if (levelText != null) levelText.text = $"Уровень: {currentLevel} / {maxLevel}";

        bool hasNextLevel = currentLevel < maxLevel;

        if (valueText != null)
        {
            float currentValue = upgrade.Stat.Value;
            if (hasNextLevel)
            {
                float nextDelta = upgrade.Levels[currentLevel].DeltaUpgradeValue;
                float nextValue = currentValue + nextDelta;
                valueText.text = $"{currentValue} → {nextValue}";
            }
            else
            {
                valueText.text = $"{currentValue} (Макс.)";
            }
        }

        ClearPriceContainer();

        if (hasNextLevel)
        {
            var prices = upgrade.Levels[currentLevel].Price;

            if (prices == null || prices.Count == 0)
            {
                if (infoPriceText != null)
                {
                    infoPriceText.gameObject.SetActive(true);
                    infoPriceText.text = "Бесплатно";
                }
            }
            else
            {
                if (infoPriceText != null) infoPriceText.gameObject.SetActive(false);

                foreach (var cost in prices)
                {
                    if (pricePrefab != null && priceContainer != null)
                    {
                        PriceElement element = Instantiate(pricePrefab, priceContainer);
                        bool canAfford = cost.Currency.Amount >= cost.Amount;
                        element.Setup(cost.Currency.Icon, cost.Amount, canAfford);
                    }
                }
            }
        }
        else
        {
            if (infoPriceText != null)
            {
                infoPriceText.gameObject.SetActive(true);
                infoPriceText.text = "Максимальный уровень";
            }
        }

        // КРИТИЧЕСКИЙ СТЁБ НАД UNITY UI:
        // Принудительно заставляем Canvas пересчитать размерыLayout-групп ПОСЛЕ спавна иконок цен,
        // иначе тултип посчитает позицию по старым (нулевым) размерам.
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        if (followMouse) UpdatePosition();
    }

    public void Hide()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void ClearPriceContainer()
    {
        if (priceContainer == null) return;
        foreach (Transform child in priceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdatePosition()
    {
        if (Mouse.current == null || parentCanvas == null || parentRectTransform == null) return;

        // 1. Получаем позицию мыши из нового Input System
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // 2. Переводим экранные пиксели мыши в локальные координаты UI
        Camera cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform, 
            mousePosition, 
            cam, 
            out Vector2 localPoint))
        {
            // 3. Устанавливаем корректную позицию через anchoredPosition
            rectTransform.anchoredPosition = localPoint + offset;
        }
    }
}