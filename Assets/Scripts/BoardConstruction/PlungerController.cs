using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlungerController : MonoBehaviour
{
    public UnityEvent OnLaunch;

    [Header("Main Settings")]
    [SerializeField] private string launchActionName = "Plunger";
    [SerializeField] private float maxForce = 15f;
    [SerializeField] private float pullSpeed = 3f;
    [SerializeField] private float releaseSpeed = 20f;

    [Header("Visuals & Squish")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float minSquishScaleY = 0.5f;

    [Header("Ball Detection")]
    [SerializeField] private Vector2 detectionBoxSize = new(0.45f, 2f);
    [SerializeField] private Vector2 detectionOffset = new(0f, 1f);
    [SerializeField] private LayerMask ballLayer;

    private Rigidbody2D _plungerRigidbody;
    private Vector2 _originalPosition;

    private bool _isCharging;
    private bool _wasPressedLastFrame;

    private float _chargeRatio;

    private InputAction _launchAction;

    private Collider2D _visualCollider;

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

        if (_launchAction == null)
            return;

        bool isPressed = _launchAction.IsPressed();

        if (isPressed && !_wasPressedLastFrame)
        {
            _isCharging = true;
        }
        else if (!isPressed && _wasPressedLastFrame)
        {
            if (_isCharging)
            {
                Launch();
            }
        }

        _wasPressedLastFrame = isPressed;

        if (_isCharging)
        {
            _chargeRatio = Mathf.MoveTowards(
                _chargeRatio,
                1f,
                pullSpeed * Time.fixedDeltaTime);
        }
        else
        {
            _chargeRatio = Mathf.MoveTowards(
                _chargeRatio,
                0f,
                releaseSpeed * Time.fixedDeltaTime);
        }

        SnapBallToPlunger();

        UpdateVisualSquish();
    }

    private Collider2D FindBall()
    {
        Vector2 center = _originalPosition + detectionOffset;

        return Physics2D.OverlapBox(
            center,
            detectionBoxSize,
            0f,
            ballLayer);
    }

    private void SnapBallToPlunger()
    {
        if (_visualCollider == null)
            return;

        Collider2D hitCollider = FindBall();

        if (hitCollider == null || !hitCollider.CompareTag("Ball"))
            return;

        Rigidbody2D ballRb = hitCollider.GetComponent<Rigidbody2D>();

        if (ballRb == null)
            return;

        // Уже вылетел
        if (ballRb.linearVelocity.y > 0.1f)
            return;

        // Не магнитим боковые шары
        float xDistance = Mathf.Abs(
            ballRb.position.x - _originalPosition.x);

        if (xDistance > detectionBoxSize.x * 0.5f)
            return;

        float ballRadius = hitCollider.bounds.extents.y;

        float targetY = _visualCollider.bounds.max.y + ballRadius;

        ballRb.linearVelocity = Vector2.zero;

        ballRb.MovePosition(new Vector2(
            _originalPosition.x,
            targetY));
    }

    private void Launch()
    {
        _isCharging = false;

        float appliedForce = maxForce * _chargeRatio;

        _chargeRatio = 0f;

        if (appliedForce <= 0.05f)
            return;

        Collider2D hitCollider = FindBall();

        if (hitCollider == null || !hitCollider.CompareTag("Ball"))
            return;

        Rigidbody2D ballRb = hitCollider.GetComponent<Rigidbody2D>();

        if (ballRb == null)
            return;

        ballRb.linearVelocity = Vector2.zero;

        ballRb.AddForce(
            Vector2.up * appliedForce,
            ForceMode2D.Impulse);

        OnLaunch?.Invoke();
    }

    private void UpdateVisualSquish()
    {
        if (visualTransform == null)
            return;

        float scaleY = Mathf.Lerp(
            1f,
            minSquishScaleY,
            _chargeRatio);

        visualTransform.localScale = new Vector3(
            visualTransform.localScale.x,
            scaleY,
            visualTransform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector2 center;

        if (Application.isPlaying)
        {
            center = _originalPosition + detectionOffset;
        }
        else
        {
            center = (Vector2)transform.position + detectionOffset;
        }

        Gizmos.DrawWireCube(center, detectionBoxSize);
    }
}