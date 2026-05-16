using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlungerController : MonoBehaviour
{
    [SerializeField] private float maxForce = 150f;
    [SerializeField] private float maxPullDistance = 1.2f;
    [SerializeField] private float pullSpeed = 3f;
    [SerializeField] private float releaseSpeed = 40f;
    [SerializeField] private InputActionReference launchActionReference;

    private Rigidbody2D _plungerRigidbody;
    private Rigidbody2D _ballRigidbody;
    private Vector2 _originalPosition;
    private bool _isCharging;

    private void Start()
    {
        _plungerRigidbody = GetComponent<Rigidbody2D>();
        _originalPosition = _plungerRigidbody.position;
    }

    private void OnEnable()
    {
        if (launchActionReference == null) return;
        launchActionReference.action.started += OnLaunchStarted;
        launchActionReference.action.canceled += OnLaunchCanceled;
        launchActionReference.action.Enable();
    }

    private void OnDisable()
    {
        if (launchActionReference == null) return;
        launchActionReference.action.started -= OnLaunchStarted;
        launchActionReference.action.canceled -= OnLaunchCanceled;
        launchActionReference.action.Disable();
    }

    private void FixedUpdate()
    {
        if (_isCharging)
        {
            Vector2 targetPosition = 
                _originalPosition + Vector2.down * maxPullDistance;
            var newPosition = 
                Vector2.MoveTowards(_plungerRigidbody.position, targetPosition, 
                    pullSpeed * Time.fixedDeltaTime);
            _plungerRigidbody.MovePosition(newPosition);
        }
        else
        {
            var newPosition = 
                Vector2.MoveTowards(_plungerRigidbody.position, _originalPosition, 
                    releaseSpeed * Time.fixedDeltaTime);
            _plungerRigidbody.MovePosition(newPosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _ballRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _ballRigidbody = null;
        }
    }

    private void OnLaunchStarted(InputAction.CallbackContext context)
    {
        _isCharging = true;
    }

    private void OnLaunchCanceled(InputAction.CallbackContext context)
    {
        if (_isCharging)
        {
            Launch();
        }
    }

    private void Launch()
    {
        _isCharging = false;

        float pullDistance = Vector2.Distance(_plungerRigidbody.position, _originalPosition);
        float pullRatio = pullDistance / maxPullDistance;
        float appliedForce = maxForce * pullRatio;

        if (_ballRigidbody == null || !(pullRatio > 0.05f)) return;
        _ballRigidbody.linearVelocity = Vector2.zero;
        _ballRigidbody.AddForce(Vector2.up * appliedForce, ForceMode2D.Impulse);
    }
}