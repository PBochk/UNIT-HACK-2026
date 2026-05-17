using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class GhostController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float shiftVerticalDistance = 1f;
    [SerializeField] private float shiftSpeed = 5f;
    [SerializeField] private LayerMask wallLayer; // Слой стен, пола и потолка
    [SerializeField] private float wallOffset = 0.2f; // Зазор до стен

    
    [SerializeField] private int bonusScores = 50;
    [SerializeField] private Currency currency;
    
    private int _horizontalDirection = 1;
    private int _verticalShiftDirection = 1;
    private bool _isShiftingDown;
    private Vector3 _targetPosition;

    // Границы теперь рассчитываются автоматически
    private float _leftBound;
    private float _rightBound;
    private float _upperBoundY;
    private float _lowerBoundY;

    // Флаг паузы для фазы расстановки
    private bool _isPaused;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        DetectBounds();
    }

    // --- ПУБЛИЧНЫЙ МЕТОД ДЛЯ ПАУЗЫ ---
    public void SetPaused(bool isPaused)
    {
        _isPaused = isPaused;
    }

    private void DetectBounds()
    {
        Collider2D col = GetComponent<Collider2D>();
        float extentsX = col != null ? col.bounds.extents.x : 0.5f;
        float extentsY = col != null ? col.bounds.extents.y : 0.5f;

        // Лучевой поиск влево
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 100f, wallLayer);
        _leftBound = hitLeft.collider != null ? hitLeft.point.x + extentsX + wallOffset : transform.position.x - 5f;

        // Лучевой поиск вправо
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 100f, wallLayer);
        _rightBound = hitRight.collider != null ? hitRight.point.x - extentsX - wallOffset : transform.position.x + 5f;

        // Лучевой поиск вверх (потолок)
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 100f, wallLayer);
        _upperBoundY = hitUp.collider != null ? hitUp.point.y - extentsY - wallOffset : transform.position.y + 5f;

        // Лучевой поиск вниз (пол)
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 100f, wallLayer);
        _lowerBoundY = hitDown.collider != null ? hitDown.point.y + extentsY + wallOffset : transform.position.y - 5f;
    }

    private void FixedUpdate()
    {
        // Если на паузе — не двигаемся
        if (_isPaused) return;

        MoveSnake();
    }

    private void MoveSnake()
    {
        if (_isShiftingDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, shiftSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                transform.position = _targetPosition;
                _isShiftingDown = false;

                if (_verticalShiftDirection == 1 && transform.position.y <= _lowerBoundY)
                {
                    _verticalShiftDirection = -1;
                }
                else if (_verticalShiftDirection == -1 && transform.position.y >= _upperBoundY)
                {
                    _verticalShiftDirection = 1;
                }
            }
            return;
        }

        transform.Translate(Vector3.right * _horizontalDirection * moveSpeed * Time.fixedDeltaTime);

        if ((_horizontalDirection == 1 && transform.position.x >= _rightBound) ||
            (_horizontalDirection == -1 && transform.position.x <= _leftBound))
        {
            Vector3 shiftDirection = _verticalShiftDirection == 1 ? Vector3.down : Vector3.up;
            _targetPosition = transform.position + shiftDirection * shiftVerticalDistance;
            
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, _lowerBoundY, _upperBoundY);

            _isShiftingDown = true;
            _horizontalDirection *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Игнорируем столкновения с мячом во время паузы фазы расстановки
        if (_isPaused) return;

        if (!collider.CompareTag("Ball")) return;

        BallController ballState = collider.GetComponent<BallController>();
        Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();

        if (ballState != null && ballState.IsPoweredUp)
        {
            Destroy(gameObject);
            currency.Amount += bonusScores;
        }
        else if (ballRb != null)
        {
            ballRb.linearVelocity *= 0.5f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        
        float left = _leftBound;
        float right = _rightBound;
        float top = _upperBoundY;
        float bottom = _lowerBoundY;

        if (!Application.isPlaying)
        {
            Collider2D col = GetComponent<Collider2D>();
            float extentsX = col != null ? col.bounds.extents.x : 0.5f;
            float extentsY = col != null ? col.bounds.extents.y : 0.5f;

            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 100f, wallLayer);
            left = hitLeft.collider != null ? hitLeft.point.x + extentsX + wallOffset : transform.position.x - 3f;

            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 100f, wallLayer);
            right = hitRight.collider != null ? hitRight.point.x - extentsX - wallOffset : transform.position.x + 3f;

            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 100f, wallLayer);
            top = hitUp.collider != null ? hitUp.point.y - extentsY - wallOffset : transform.position.y + 3f;

            RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 100f, wallLayer);
            bottom = hitDown.collider != null ? hitDown.point.y + extentsY + wallOffset : transform.position.y - 3f;
        }

        Vector3 topLeft = new (left, top, 0);
        Vector3 topRight = new (right, top, 0);
        Vector3 bottomLeft = new (left, bottom, 0);
        Vector3 bottomRight = new (right, bottom, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}