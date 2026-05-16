using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class GhostController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float shiftVerticalDistance = 1f;
    [SerializeField] private float shiftSpeed = 5f;
    [SerializeField] private float leftBound = -5f;
    [SerializeField] private float rightBound = 5f;
    [SerializeField] private float upperBoundY = 5f;
    [SerializeField] private float lowerBoundY = -5f;

    private int _horizontalDirection = 1;
    private int _verticalShiftDirection = 1;
    private bool _isShiftingDown;
    private Vector3 _targetPosition;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void FixedUpdate()
    {
        MoveSnake();
    }

    private void MoveSnake()
    {
        if (_isShiftingDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, shiftSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                transform.position = _targetPosition;
                _isShiftingDown = false;

                if (_verticalShiftDirection == 1 && transform.position.y <= lowerBoundY)
                {
                    _verticalShiftDirection = -1;
                }
                else if (_verticalShiftDirection == -1 && transform.position.y >= upperBoundY)
                {
                    _verticalShiftDirection = 1;
                }
            }
            return;
        }

        transform.Translate(Vector3.right * _horizontalDirection * moveSpeed * Time.deltaTime);

        if ((_horizontalDirection == 1 && transform.position.x >= rightBound) ||
            (_horizontalDirection == -1 && transform.position.x <= leftBound))
        {
            Vector3 shiftDirection = _verticalShiftDirection == 1 ? Vector3.down : Vector3.up;
            _targetPosition = transform.position + shiftDirection * shiftVerticalDistance;
            _isShiftingDown = true;
            _horizontalDirection *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball")) return;

        BallController ballState = collider.GetComponent<BallController>();
        Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();

        if (ballState != null && ballState.IsPoweredUp)
        {
            Destroy(gameObject);
        }
        else if (ballRb != null)
        {
            ballRb.linearVelocity *= 0.5f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 topLeft = new (leftBound, upperBoundY, 0);
        Vector3 topRight = new (rightBound, upperBoundY, 0);
        Vector3 bottomLeft = new (leftBound, lowerBoundY, 0);
        Vector3 bottomRight = new (rightBound, lowerBoundY, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}