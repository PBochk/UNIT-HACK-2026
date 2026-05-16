using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // ОБЯЗАТЕЛЬНО: подключаем новый Input System

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
    [SerializeField] private Vector3 offset = new Vector3(15f, -15f, 0f); 

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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

        if (levelText != null) levelText.text = $"{currentLevel} / {maxLevel}";

        bool hasNextLevel = currentLevel < maxLevel;

        if (valueText != null)
        {
            float currentValue = upgrade.Stat.Value;
            if (hasNextLevel)
            {
                float nextDelta = upgrade.Levels[currentLevel].DeltaUpgradeValue;
                float nextValue = currentValue + nextDelta;
                valueText.text = $"{currentValue} → <color=#00FF00>{nextValue}</color>";
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
                infoPriceText.text = "<color=#FFD700>Максимальный уровень</color>";
            }
        }

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

    // ИЗМЕНЕНО: Логика отслеживания мыши через New Input System
    private void UpdatePosition()
    {
        // Проверяем, подключена ли мышь в данный момент
        if (Mouse.current == null) return;

        // Считываем позицию мыши в экранных координатах
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        // Устанавливаем позицию тултипа с учетом смещения
        transform.position = (Vector3)mousePosition + offset;
    }
}