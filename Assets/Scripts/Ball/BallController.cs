using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class BallController : MonoBehaviour
{
    [Header("Visual PowerUp Settings")]
    [SerializeField] private Sprite powerUpSprite; // Спрайт усиленного мяча

    public bool IsPoweredUp { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private Sprite _normalSprite; // Сюда автоматически сохранится базовый спрайт
    private Coroutine _powerUpRoutine;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Запоминаем дефолтный спрайт, который сейчас стоит на мяче
        if (_spriteRenderer != null)
        {
            _normalSprite = _spriteRenderer.sprite;
        }
    }

    public void ActivatePowerUp(float duration)
    {
        // Защита: если повер-ап уже активен, сбрасываем старый таймер и запускаем заново
        if (_powerUpRoutine != null)
        {
            StopCoroutine(_powerUpRoutine);
        }
        
        _powerUpRoutine = StartCoroutine(PowerUpRoutine(duration));
    }

    private IEnumerator PowerUpRoutine(float duration)
    {
        IsPoweredUp = true;

        // Включаем спрайт повер-апа
        if (_spriteRenderer != null && powerUpSprite != null)
        {
            _spriteRenderer.sprite = powerUpSprite;
        }

        yield return new WaitForSeconds(duration);

        IsPoweredUp = false;

        // Возвращаем обычный спрайт назад
        if (_spriteRenderer != null && _normalSprite != null)
        {
            _spriteRenderer.sprite = _normalSprite;
        }

        _powerUpRoutine = null;
    }
}