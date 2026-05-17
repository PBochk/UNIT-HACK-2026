using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class BumperController : MonoBehaviour
{
    [Header("Bounciness Settings")]
    [SerializeField] private float bounceForce = 15f;
    
    [Range(0f, 360f)] 
    [SerializeField] private float bounceAngle = 90f;

    [Header("Visuals Settings")]
    [SerializeField] private Sprite simple;
    [SerializeField] private Sprite bounce;
    [Tooltip("Время отображения спрайта удара в секундах")]
    [SerializeField] private float visualHoldTime = 0.1f; 

    private SpriteRenderer _spriteRenderer;
    private Coroutine _visualRoutine;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Устанавливаем дефолтный спрайт на старте, если он назначен
        if (simple != null)
        {
            _spriteRenderer.sprite = simple;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            Vector2 launchDirection = GetVectorFromAngle(bounceAngle);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(launchDirection * bounceForce, ForceMode2D.Impulse);

            // Запускаем визуальный эффект изменения спрайта
            TriggerBounceVisual();
        }
    }

    private void TriggerBounceVisual()
    {
        if (_spriteRenderer == null || bounce == null || simple == null) return;

        // Если бампер еще не успел вернуться в обычное состояние от прошлого удара,
        // прерываем старый таймер и запускаем заново
        if (_visualRoutine != null)
        {
            StopCoroutine(_visualRoutine);
        }

        _visualRoutine = StartCoroutine(BounceVisualRoutine());
    }

    private IEnumerator BounceVisualRoutine()
    {
        _spriteRenderer.sprite = bounce;

        // Ждем указанное количество времени (например, 0.1 секунды — это около 6 кадров при 60 FPS)
        yield return new WaitForSeconds(visualHoldTime);

        _spriteRenderer.sprite = simple;
        _visualRoutine = null;
    }

    private Vector2 GetVectorFromAngle(float angleInDegrees)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var direction = (Vector3)GetVectorFromAngle(bounceAngle);
        Vector3 startPosition = transform.position;
        
        Vector3 endPosition = startPosition + direction * (bounceForce * 0.1f);
        Gizmos.DrawLine(startPosition, endPosition);
        
        Gizmos.DrawWireSphere(endPosition, 0.1f);
    }
}