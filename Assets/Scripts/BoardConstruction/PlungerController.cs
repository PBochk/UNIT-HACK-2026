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

    private Collider2D _visualCollider;
    private float _ballRadius = -1f; 

    // Ссылка на Rigidbody удерживаемого шара
    private Rigidbody2D _currentBallRb;

    private void Start()
    {
        _plungerRigidbody = GetComponent<Rigidbody2D>();
        _originalPosition = _plungerRigidbody.position;
        _launchAction = InputManager.Instance.GetAction(launchActionName);

        if (visualTransform != null)
        {
            _visualCollider = visualTransform.GetComponent<Collider2D>();
        }
    }

    private void FixedUpdate()
    {
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
        }
        else
        {
            _chargeRatio = Mathf.MoveTowards(_chargeRatio, 0f, releaseSpeed * Time.fixedDeltaTime);
        }

        // ИЗМЕНЕНИЕ: Метод удержания шара теперь работает КАЖДЫЙ кадр в FixedUpdate,
        // гарантируя, что шар не провалится ни при падении, ни при зарядке.
        HoldBallOnTop();

        UpdateVisualSquish();
    }

    private void HoldBallOnTop()
    {
        if (visualTransform == null || _visualCollider == null) return;

        // Определяем динамический центр зоны удержания с учетом текущего сжатия
        float currentPlungerHeightOffset = launchZoneOffset.y * Mathf.Lerp(1f, minSquishScaleY, _chargeRatio);
        Vector2 scanPosition = _originalPosition + Vector2.up * (currentPlungerHeightOffset + launchZoneRadius);

        // Сканируем компактную зону над текущей макушкой коллайдера
        Collider2D hitCollider = Physics2D.OverlapCircle(scanPosition, launchZoneRadius, ballLayer);

        if (hitCollider != null && hitCollider.CompareTag("Ball"))
        {
            // Фильтр по X: Игнорируем мячи в соседних накопителях
            if (Mathf.Abs(hitCollider.transform.position.x - _originalPosition.x) > launchZoneRadius) return;

            // Кэшируем Rigidbody шара для постоянного использования
            if (_currentBallRb == null || _currentBallRb.gameObject != hitCollider.gameObject)
            {
                _currentBallRb = hitCollider.GetComponent<Rigidbody2D>();
                if (_currentBallRb != null)
                {
                    _ballRadius = hitCollider.bounds.extents.y;
                }
            }

            if (_currentBallRb != null)
            {
                // ИДЕАЛЬНО точное положение верхней границы коллайдера пружины
                float plungerTopY = _visualCollider.bounds.max.y;
                float targetBallY = plungerTopY + _ballRadius;

                // Принудительное удержание физическим перемещением
                _currentBallRb.linearVelocity = new Vector2(_currentBallRb.linearVelocity.x, 0f);
                _currentBallRb.MovePosition(new Vector2(_currentBallRb.position.x, targetBallY));
            }
        }
        else
        {
            // Если шар улетел или отсутствует, сбрасываем кэш
            _currentBallRb = null;
        }
    }

    private void Launch()
    {
        _isCharging = false;
        
        float appliedForce = maxForce * _chargeRatio;
        float finalCharge = _chargeRatio;
        
        _chargeRatio = 0f;

        if (finalCharge <= 0.05f) return;
        
        // Для запуска используем текущий кэшированный шар, если он есть
        if (_currentBallRb != null)
        {
            _currentBallRb.linearVelocity = Vector2.zero;
            _currentBallRb.AddForce(Vector2.up * appliedForce, ForceMode2D.Impulse);
            
            // Сразу сбрасываем кэш, чтобы метод Hold не притягивал его обратно в этом кадре
            _currentBallRb = null; 
            _ballRadius = -1f; 
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
        Vector2 center = (Application.isPlaying && _visualCollider != null) 
            ? new Vector2(_originalPosition.x, _visualCollider.bounds.max.y + launchZoneRadius) 
            : (Vector2)transform.position + launchZoneOffset;
            
        Gizmos.DrawWireSphere(center, launchZoneRadius);
    }
}