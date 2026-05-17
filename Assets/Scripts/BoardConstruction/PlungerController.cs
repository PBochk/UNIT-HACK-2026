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
        if (visualTransform == null || _visualCollider == null) return;

        // ИЗМЕНЕНИЕ 1: Сканируем компактную зону, которая привязана строго к ТЕКУЩЕЙ верхушке пружины
        Vector2 scanPosition = new Vector2(_originalPosition.x, _visualCollider.bounds.max.y + launchZoneRadius);
        Collider2D hitCollider = Physics2D.OverlapCircle(scanPosition, launchZoneRadius, ballLayer);

        if (hitCollider != null && hitCollider.CompareTag("Ball"))
        {
            // ИЗМЕНЕНИЕ 2: Защита от соседних линий. Если мяч смещен по X дальше своего радиуса — игнорируем
            if (Mathf.Abs(hitCollider.transform.position.x - _originalPosition.x) > launchZoneRadius) return;

            Rigidbody2D ballRb = hitCollider.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                if (_ballRadius < 0f)
                {
                    _ballRadius = hitCollider.bounds.extents.y;
                }

                float plungerTopY = _visualCollider.bounds.max.y;
                float targetBallY = plungerTopY + _ballRadius;

                ballRb.linearVelocity = new Vector2(ballRb.linearVelocity.x, 0f);
                ballRb.MovePosition(new Vector2(ballRb.position.x, targetBallY));
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
        
        if (_visualCollider != null)
        {
            // ИЗМЕНЕНИЕ 3: Здесь тоже сузили зону поиска до точных размеров разгонного трека плунжера
            Vector2 scanPosition = new Vector2(_originalPosition.x, _visualCollider.bounds.max.y + launchZoneRadius);
            Collider2D hitCollider = Physics2D.OverlapCircle(scanPosition, launchZoneRadius, ballLayer);

            if (hitCollider != null && hitCollider.CompareTag("Ball"))
            {
                // Дублируем проверку по оси X для предотвращения ложного запуска соседнего мяча
                if (Mathf.Abs(hitCollider.transform.position.x - _originalPosition.x) > launchZoneRadius) return;

                Rigidbody2D ballRb = hitCollider.GetComponent<Rigidbody2D>();
                if (ballRb != null)
                {
                    ballRb.linearVelocity = Vector2.zero;
                    ballRb.AddForce(Vector2.up * appliedForce, ForceMode2D.Impulse);
                    _ballRadius = -1f; 
                }
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
        Vector2 center = (Application.isPlaying && _visualCollider != null) 
            ? new Vector2(_originalPosition.x, _visualCollider.bounds.max.y + launchZoneRadius) 
            : (Vector2)transform.position + launchZoneOffset;
            
        Gizmos.DrawWireSphere(center, launchZoneRadius);
    }
}