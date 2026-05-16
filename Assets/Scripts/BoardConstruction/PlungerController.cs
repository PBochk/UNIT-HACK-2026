using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlungerController : MonoBehaviour
{
    [SerializeField] private string launchActionName = "Plunger";
    [SerializeField] private float maxForce = 150f;
    [SerializeField] private float maxPullDistance = 1.2f;
    [SerializeField] private float pullSpeed = 3f;
    [SerializeField] private float releaseSpeed = 40f;

    private Rigidbody2D _plungerRigidbody;
    private Rigidbody2D _ballRigidbody;
    private Vector2 _originalPosition;
    private bool _isCharging;
    private bool _wasPressedLastFrame;
    private InputAction _launchAction;

    private void Start()
    {
        _plungerRigidbody = GetComponent<Rigidbody2D>();
        _originalPosition = _plungerRigidbody.position;
        _launchAction = InputManager.Instance.GetAction(launchActionName);
    }

    private void FixedUpdate()
    {
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
            Vector2 targetPosition = _originalPosition + Vector2.down * maxPullDistance;
            Vector2 newPosition = Vector2.MoveTowards(_plungerRigidbody.position, targetPosition, pullSpeed * Time.fixedDeltaTime);
            _plungerRigidbody.MovePosition(newPosition);
        }
        else
        {
            Vector2 newPosition = Vector2.MoveTowards(_plungerRigidbody.position, _originalPosition, releaseSpeed * Time.fixedDeltaTime);
            _plungerRigidbody.MovePosition(newPosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            _ballRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            _ballRigidbody = null;
    }

    private void Launch()
    {
        _isCharging = false;
        float pullDistance = Vector2.Distance(_plungerRigidbody.position, _originalPosition);
        float pullRatio = pullDistance / maxPullDistance;
        float appliedForce = maxForce * pullRatio;

        if (_ballRigidbody == null || pullRatio <= 0.05f) return;
        _ballRigidbody.linearVelocity = Vector2.zero;
        _ballRigidbody.AddForce(Vector2.up * appliedForce, ForceMode2D.Impulse);
    }
}