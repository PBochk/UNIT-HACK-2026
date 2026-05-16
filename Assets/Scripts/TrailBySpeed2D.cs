using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TrailBySpeed2D : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    [SerializeField] private TrailRenderer trailRenderer;
    
    [Header("Настройки порога скорости")]
    [Tooltip("Скорость, при превышении которой включается Trail")]
    [SerializeField] private float speedThreshold = 5f;

    private Rigidbody2D rb;

    private void Start()
    {
        // Получаем ссылку на Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Если TrailRenderer не назначен в инспекторе, пробуем найти его на этом же объекте
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        // Проверка на случай, если компонент так и не найден
        if (trailRenderer == null)
        {
            Debug.LogError($"На объекте {gameObject.name} не найден TrailRenderer!", this);
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
        }
    }

    private void FixedUpdate()
    {
        // Получаем текущую скорость объекта (величину вектора)
        float currentSpeed = rb.linearVelocity.magnitude;

        // Включаем или выключаем эмиссию хвоста в зависимости от скорости
        if (currentSpeed >= speedThreshold)
        {
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }
    }
}