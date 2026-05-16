using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PriceElement : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    public void Setup(Sprite icon, int amount, bool canAfford)
    {
        if (iconImage != null) 
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(icon != null); // Скрываем картинку, если иконки нет
        }

        if (amountText != null)
        {
            // Белый цвет, если хватает денег, красный — если нет
            string colorHex = canAfford ? "#FFFFFF" : "#FF4D4D";
            amountText.text = $"<color={colorHex}>{amount}</color>";
        }
    }
}