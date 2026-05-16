using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    [Tooltip("Физический объект, за чьей скоростью мы следим")]
    [SerializeField] private Rigidbody2D targetRigidbody; 
    
    [Tooltip("Компонент TextMeshPro (UGUI) для вывода текста")]
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Настройки отображения")]
    [SerializeField] private string prefix = "Скорость: ";
    [SerializeField] private string suffix = " м/с";
    
    [Tooltip("Включите, чтобы автоматически переводить скорость из м/с в км/ч")]
    [SerializeField] private bool convertToKmH = false;
    
    [Tooltip("Количество знаков после запятой (например: F0 - целые, F1 - один знак, F2 - два)")]
    [SerializeField] private string floatFormat = "F1";

    void Update()
    {
        if (targetRigidbody == null || speedText == null) return;

        float currentSpeed = targetRigidbody.linearVelocity.magnitude;

        if (convertToKmH)
        {
            currentSpeed *= 3.6f;
        }

        speedText.text = $"{prefix}{currentSpeed.ToString(floatFormat)}{suffix}";
    }
}