using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlungerController : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private string launchActionName = "Plunger";
    [SerializeField] private float maxForce = 15f;
    [SerializeField] private float pullSpeed = 3f;
    [SerializeField] private float releaseSpeed = 20f;

    [Header("Visuals & Squish")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float minSquishScaleY = 0.5f;

    [Header("Ball Detection Zone")]
    [SerializeField] private Vector2 launchZoneOffset = Vector2.up * 0.5f;
    [SerializeField] private float launchZoneRadius = 0.4f;
    [SerializeField] private LayerMask ballLayer;

    private Rigidbody2D _plungerRigidbody;
    private Vector2 _originalPosition;
    private bool _isCharging;
    private bool _wasPressedLastFrame;
    private InputAction _launchAction;
    private float _chargeRatio; 

    private void Start()
    {
        _plungerRigidbody = GetComponent<Rigidbody2D>();
        _originalPosition = _plungerRigidbody.position;
        _launchAction = InputManager.Instance.GetAction(launchActionName);
    }

    private void FixedUpdate()
    {
        // Родитель всегда неподвижен
        _plungerRigidbody.MovePosition(_originalPosition);

        if (_launchAction == null) return;

        bool isPressed = _launchAction.IsPressed();

        if (isPressed && !_wasPressedLastFrame)
        {
            _isCharging = true;
        }
        else if (!isPressed && _wasPressedLastFrame)
        {
            if (_isCharging)
                Launch();
        }

        _wasPressedLastFrame = isPressed;

        if (_isCharging)
        {
            _chargeRatio = Mathf.MoveTowards(_chargeRatio, 1f, pullSpeed * Time.fixedDeltaTime);
            
            // ИСПРАВЛЕНИЕ: Принудительно удерживаем шар на сжимающейся поверхности
            SnapBallToVisualTop();
        }
        else
        {
            _chargeRatio = Mathf.MoveTowards(_chargeRatio, 0f, releaseSpeed * Time.fixedDeltaTime);
        }

        UpdateVisualSquish();
    }

    private void SnapBallToVisualTop()
    {
        if (visualTransform == null) return;

        // Ищем шар в зоне плунжера
        Vector2 scanPosition = _originalPosition + launchZoneOffset;
        Collider2D hitCollider = Physics2D.OverlapCircle(scanPosition, launchZoneRadius, ballLayer);

        if (hitCollider != null && hitCollider.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = hitCollider.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                // Находим текущую верхнюю точку сжимающегося визуала
                // Для этого берем исходное смещение зоны и умножаем его Y на текущий масштаб сжатия
                float currentScaleY = Mathf.Lerp(1f, minSquishScaleY, _chargeRatio);
                
                // Плавно опускаем скорость шара до нуля по Y, чтобы он не пробивал коллайдер
                if (ballRb.linearVelocity.y < 0)
                {
                    ballRb.linearVelocity = new Vector2(ballRb.linearVelocity.x, 0f);
                }
            }
        }
    }

    private void Launch()
    {
        _isCharging = false;
        
        float appliedForce = maxForce * _chargeRatio;
        float finalCharge = _chargeRatio;
        
        _chargeRatio = 0f;

        if (finalCharge <= 0.05f) return;
        
        // Сканируем зону с учетом текущего сжатия
        Vector2 scanPosition = _originalPosition + launchZoneOffset * Mathf.Lerp(1f, minSquishScaleY, finalCharge);
        Collider2D hitCollider = Physics2D.OverlapCircle(scanPosition, launchZoneRadius, ballLayer);

        if (hitCollider != null && hitCollider.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = hitCollider.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector2.zero;
                ballRb.AddForce(Vector2.up * appliedForce, ForceMode2D.Impulse);
            }
        }
    }

    private void UpdateVisualSquish()
    {
        if (visualTransform == null) return;
        
        float currentScaleY = Mathf.Lerp(1f, minSquishScaleY, _chargeRatio);
        visualTransform.localScale = new Vector3(visualTransform.localScale.x, currentScaleY, visualTransform.localScale.z);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        // Отображаем зону сканирования в зависимости от текущего сжатия в игре
        float currentScaleY = Application.isPlaying ? Mathf.Lerp(1f, minSquishScaleY, _chargeRatio) : 1f;
        Vector2 center = (Application.isPlaying ? _originalPosition : (Vector2)transform.position) + launchZoneOffset * currentScaleY;
        Gizmos.DrawWireSphere(center, launchZoneRadius);
    }
}